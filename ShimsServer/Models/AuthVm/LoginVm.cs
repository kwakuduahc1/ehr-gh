using System.ComponentModel.DataAnnotations;

namespace ShimsServer.Models.AuthVm
{
    public record LoginVm(
        [Required, StringLength(100)] string Email, 
        [Required, StringLength(15, MinimumLength = 6)] string Password);
}
