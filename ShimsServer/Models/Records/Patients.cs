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

        [Required]
        public required DateTime DateOfBirth { get; set; }

        [Required, StringLength(6, MinimumLength = 4), AllowedValues(["Male", "Female"])]
        public required string Sex { get; set; }

        [Required, DataType(DataType.PhoneNumber)]
        public required string PhoneNumber { get; set; }

        [Required, StringLength(75, MinimumLength = 10)]
        public required string UserName { get; set; }
    }

    public record AddPatientDto(
     [Required]
    string HospitalID,

     [Required, StringLength(30, MinimumLength = 3)]
    string Surname,

     [Required, StringLength(50, MinimumLength = 3)]
    string OtherNames,

     [Required]
    DateTime DateOfBirth,

     [Required, StringLength(6, MinimumLength = 4)]
    [AllowedValues("Male", "Female")] // assuming you have this custom attribute
    string Sex,

     [Required, DataType(DataType.PhoneNumber)]
    string PhoneNumber
 );

    public record EditPatientDto(

    [Required]
    Guid PatientID,

    [Required]
    string HospitalID,

    [Required, StringLength(30, MinimumLength = 3)]
    string Surname,

    [Required, StringLength(50, MinimumLength = 3)]
    string OtherNames,

    [Required]
    DateTime DateOfBirth,

    [Required, StringLength(6, MinimumLength = 4)]
    [AllowedValues("Male", "Female")] // assuming you have this custom attribute
    string Sex,

    [Required, DataType(DataType.PhoneNumber)]
    string PhoneNumber
);

}
