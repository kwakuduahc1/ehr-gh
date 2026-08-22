using ShimsServer.Models.Records;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShimsServer.Models.ConsultingRoom
{
    public class PatientConsultation
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid PatientConsultationID { get; set; } = Guid.CreateVersion7();

        [Required]
        [ForeignKey(nameof(PatientAttendance))]
        public required Guid PatientAttendancesID { get; set; }

        [StringLength(500, ErrorMessage = "{0} should be below {1} characters")]
        [Required(AllowEmptyStrings = false)]
        public required string Complaints { get; set; }

        [StringLength(250, ErrorMessage = "{0} should be below {1} characters")]
        public required string ODQ { get; set; }

        [Column(TypeName = "jsonb")]
        public AVPU? AVPU { get; set; }

        [Column(TypeName = "jsonb")]
        public GCS? GCS { get; set; }

        public DateTime DateAdded { get; set; }

        [Required, StringLength(75, MinimumLength = 10)]
        public required string UserName { get; set; }

        public virtual PatientAttendance? PatientAttendance { get; set; }
    }

    public record AVPU([Range(0, 4)] short? Alert, [Range(0, 3)] short? Verbal, [Range(0, 2)] short? Pain, [Range(0, 1)] short? Responsive, short? Score);

    public record GCS([Range(0, 4)] short? EyeOpening, [Range(0, 5)] short? VerbalResponse, [Range(0, 6)] short? MotorResponse, short? Score);

    public record AddPatientConsultationDto(
    Guid PatientAttendancesID,

    [StringLength(500, ErrorMessage = "{0} should be below {1} characters")]
    string Complaints,

    [StringLength(250, ErrorMessage = "{0} should be below {1} characters")]
    string ODQ,

    AVPU? AVPU,

    GCS? GCS
    );

    public record PatientConsultationDto(
        Guid PatientAttendancesID,
        string Complaints,
        string ODQ,
        DateTime DateAdded,
        string AVPU,
        string GCS
        );
}
