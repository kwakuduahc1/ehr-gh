using System.ComponentModel.DataAnnotations;

namespace ShimsServer.Models.AuthVm
{
    public record LoginVm(
        [Required, StringLength(20)] string Email, 
        [Required, StringLength(15, MinimumLength = 6)] string Password);
}
