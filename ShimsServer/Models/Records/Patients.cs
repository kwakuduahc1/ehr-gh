using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShimsServer.Models.Records
{
    public class Patients
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid PatientsID { get; set; } = Guid.CreateVersion7();

        [Required]
        public required string HospitalID { get; set; }

        [Required, StringLength(30, MinimumLength = 3)]
        public required string Surname { get; set; }

        [Required, StringLength(50, MinimumLength = 3)]
        public required string OtherNames { get; set; }

        [StringLength(17)]
        [RegularExpression(@"^GHA-\d{10}-(?:0[1-9]|1[0-6])$", ErrorMessage = "GhanaCard must follow the format: GHA-xxxxxxxxxx-01 to 16")]
        public string? GhanaCard { get; set; }

        [DefaultValue(true)]
        public bool IsActive { get; set; } = true;

        [Required]
        public required DateTime DateOfBirth { get; set; }

        [Required, StringLength(6, MinimumLength = 4), AllowedValues(["Male", "Female"])]
        public required string Sex { get; set; }

        [DataType(DataType.PhoneNumber)]
        [StringLength(15, MinimumLength = 7)]
        public string? PhoneNumber { get; set; }

        [Required, StringLength(75, MinimumLength = 10)]
        public required string UserName { get; set; }
    }
}
