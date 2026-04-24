using ShimsServer.Context;
using System.ComponentModel.DataAnnotations;

namespace ShimsServer.Models.AuthVm
{
    public class RegisterVM
    {
        [Required]
        [StringLength(100, MinimumLength = 10)]
        [DataType(DataType.EmailAddress)]
        public required string Email { get; set; }

        [StringLength(30, MinimumLength = 5)]
        [Required]
        public required string UserRole { get; set; }

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

        public ApplicationUser Transform ()=> new() { Email = Email, Password = Password, PhoneNumber = PhoneNumber, FullName = FullName, UserName = Email, ConfirmPassword = ConfirmPassword};
    }
}
