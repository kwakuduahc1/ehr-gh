using ShimsServer.Context;
using System.ComponentModel.DataAnnotations;

namespace ShimsServer.Models.AuthVm
{
    

    public class GuestsVM
    {
        [Required]
        [StringLength(20, MinimumLength = 6)]
        public required string UserName { get; set; }

        [Required]
        [StringLength(10, MinimumLength = 2)]
        public required string Title { get; set; }

        [Required]
        [StringLength(15, MinimumLength = 6)]
        public required string Password { get; set; }

        [Required]
        [StringLength(15, MinimumLength = 6)]
        [Compare("Password", ErrorMessage = "Password and Confirm Password do not match.")]
        public required string ConfirmPassword { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 5)]
        public required string FullName { get; set; }

        [DataType(DataType.PhoneNumber)]
        [StringLength(10, MinimumLength = 10)]
        public required string PhoneNumber { get; set; }

        [StringLength(150, MinimumLength = 3)]
        public string? Institution { get; set; }

        public ApplicationUser Transform() => new() { Email = UserName, Password = Password, PhoneNumber = PhoneNumber, FullName = FullName, UserName = UserName, ConfirmPassword = ConfirmPassword };
    }
}
