using ShimsServer.Models.Records;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShimsServer.Models.ConsultingRoom
{
    public class PatientOutcomes
    {
        [Key, Required]
        [ForeignKey(nameof(PatientAttendance))]
        public required Guid PatientsAttendancesID { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 4)]
        [AllowedValues(["Admit", "Continue care", "Discharge (Case completed)", "Discharg (For review)", "Discharg(Against medical advice", "Transfered (Internal)", "Transfered (External)", "Died", "Absconded"])]
        public required string Outcome { get; set; }

        [StringLength(150)]
        public string? Notes { get; set; }

        [Required]
        public required DateTime OutcomeDate { get; set; } = DateTime.UtcNow;

        public virtual PatientAttendance? PatientAttendance { get; set; }
    }
}
