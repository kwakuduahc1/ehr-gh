using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShimsServer.Models.ConsultingRoom
{
    public class PatientConsultation
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid PatientConsultationID { get; set; } = Guid.CreateVersion7();

        [Required]
        public required Guid PatientsAttendancesID { get; set; }

        [StringLength(500, ErrorMessage = "{0} should be below {1} characters")]
        [Required(AllowEmptyStrings = false)]
        public required string Complaints { get; set; }

        [StringLength(250, ErrorMessage = "{0} should be below {1} characters")]
        public required string ODQ { get; set; }

        [Required]
        [Range(3, 15)]
        public required byte AVPU { get; set; } = 14;

        public DateTime DateAdded { get; set; }

        [Required, StringLength(75, MinimumLength = 10)]
        public required string UserName { get; set; }

        public virtual Records.PatientAttendance? PatientAttendance { get; set; }
    }

    public record AddPatientConsultationDto(
    Guid PatientsAttendancesID,

    [StringLength(500, ErrorMessage = "{0} should be below {1} characters")]
    string Complaints,

    [StringLength(250, ErrorMessage = "{0} should be below {1} characters")]
    string ODQ,

    [Range(3, 15)]
    byte AVPU = 14
);
}
