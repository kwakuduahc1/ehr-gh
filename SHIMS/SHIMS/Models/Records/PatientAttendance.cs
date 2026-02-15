using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SHIMS.Models.Records
{
    public class PatientAttendance
    {
        [Key]
        public Guid PatientAttendancesID { get; set; } = Guid.CreateVersion7();

        [Required]
        public required Guid PatientSchemesID { get; set; }

        [Required]
        [StringLength(15)]
        [DefaultValue("Acute")]
        [AllowedValues(["Acute", "Review", "Follow-up"])]
        public required string VisitType { get; set; }

        public DateTime DateSeen { get; set; }

        [MaxLength(75)]
        [Required]
        public required string UserName { get; set; }

        public virtual PatientSchemes? PatientSchemes { get; set; }
    }

    public record AttendanceDto(
     Guid PatientSchemesID,

    [StringLength(15)]
    [DefaultValue("Acute")]
    [AllowedValues(["Acute", "Review", "Follow-up"])]
    string VisitType
    );
}
