using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShimsServer.Models.Records
{
    public class PatientAttendance
    {
        [Key]
        public Guid PatientAttendancesID { get; set; } = Guid.CreateVersion7();

        [Required]
        [ForeignKey(nameof(Patients))]
        public required Guid PatientsID { get; set; }

        [Required]
        [StringLength(15)]
        [DefaultValue("Acute")]
        [AllowedValues(["Acute", "Review", "Follow-up"])]
        public required string VisitType { get; set; }

        public DateTime DateSeen { get; set; }

        [DefaultValue(true)]
        public bool IsActive { get; set; } = true;

        public DateTime? DateEnded { get; set; }

        [MaxLength(75)]
        [Required]
        public required string UserName { get; set; }

        public virtual Patients? Patients { get; set; }
    }
}
