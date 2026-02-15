using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SHIMS.Models.Records
{
    public class PatientAttendance
    {
        [Key]
        public Guid PatientAttendancesID { get; set; } = Guid.CreateVersion7();

        [Required]
        public Guid PatientsID { get; set; }

        [Required]
        [StringLength(15)]
        [DefaultValue("Acute")]
        [AllowedValues(["Acute", "Review", "Follow-up"])]
        public required string VisitType { get; set; }

        public DateTime DateSeen { get; set; }

        [Display(Name = "User Name"), MaxLength(75)]
        [Required]
        public required string UserName { get; set; }

        public virtual Patients? Patients { get; set; }
    }

    public record AttendanceDto(
     Guid PatientsID,

    [StringLength(15)]
    [DefaultValue("Acute")]
    [AllowedValues(["Acute", "Review", "Follow-up"])]
    string VisitType
    );
}
