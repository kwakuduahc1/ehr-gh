using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShimsServer.Context;
using ShimsServer.Controllers.Authentication.Helpers;
using ShimsServer.Models.AuthVm;
using System.Security.Claims;
using System.Text.Json;

namespace ShimsServer.Controllers.Authentication
{
    [ApiController]
    [EnableCors("bStudioApps")]
    [Route("api/[controller]/")]
    //[Authorize(Policy = "Administrators")]
    public class AuthController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, DbContextOptions<ApplicationDbContext> contextOptions, IAppFeatures app) : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly SignInManager<ApplicationUser> _signInManager = signInManager;
        private readonly ApplicationDbContext db = new(contextOptions);
        private readonly IAppFeatures appFeatures = app;

        [HttpGet("Users")]
        public async Task<IActionResult> GetUsers()
        {
            string qry = """
                SELECT u.id, username, fullname, c.role
                FROM "AspNetUsers" u
                LEFT JOIN (
                SELECT userid, json_agg(claimvalue) role
                FROM "AspNetUserClaims" c
                WHERE claimtype = 'http://schemas.microsoft.com/ws/2008/06/identity/claims/role'
                GROUP BY userid) c  ON c.userid = u.id
                """;
            var users = await db.Database.GetDbConnection().QueryAsync<UsersDto>(qry);
            return Ok(users.Select(x => new
            {
                x.ID,
                x.UserName,
                x.FullName,
                Roles = JsonSerializer.Deserialize<string[]>(x.Role)
            }));
        }

        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody] LoginVm user)
        {
            var _user = await _userManager.FindByNameAsync(user.Email);
            if (_user == null)
                return Unauthorized(new { Message = "Invalid user name or password" });
            if (!await _userManager.CheckPasswordAsync(_user, user.Password))
                return Unauthorized();
            await _signInManager.SignInAsync(_user, false);
            var claims = await _userManager.GetClaimsAsync(_user);
            var token = new AuthHelper(claims, appFeatures).Key;
            return Ok(new { Token = token });
        }

        [HttpPost("Register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterVM reg)
        {
            if (reg.Password != reg.ConfirmPassword)
                return BadRequest(new { Message = "Password and Confirm Password do not match." });
            var user = reg.Transform();
            if (await _userManager.FindByNameAsync(user.UserName!) != null)
                return BadRequest(new { Message = "User with this email already exists" });
            if (reg.PhoneNumber != null && await _userManager.Users.AnyAsync(x => x.PhoneNumber == reg.PhoneNumber))
                return BadRequest(new { Message = "User with this phone number already exists" });
            var result = await _userManager.CreateAsync(user, user.Password);
            if (!result.Succeeded)
                return BadRequest(new { Message = result.Errors.First().Description });
            if (await db.Users.CountAsync()  < 3)
                await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, "Administrator"));
            await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Name, reg.Email));
            await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, "User"));
            await _userManager.AddClaimAsync(user, new Claim(ClaimTypes.Role, reg.UserRole));
            await _userManager.AddClaimAsync(user, new Claim("UsersID", user.Id.ToString()));
            await _userManager.AddClaimAsync(user, new Claim("FullName", reg.FullName));
            await db.SaveChangesAsync();
            return Accepted(user.Id);
        }

        [HttpPost("Signout")]
        [Authorize]
        public new async Task<IActionResult> SignOut()
        {
            await _signInManager.SignOutAsync();
            return Accepted();
        }
    }
}