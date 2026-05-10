using ShimsServer.Models.Records;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShimsServer.Models.ConsultingRoom
{
    public class PatientOutcomes
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid PatientOutcomesID { get; set; } = Guid.CreateVersion7();

        [Required]
        [ForeignKey(nameof(PatientAttendance))]
        public required Guid PatientsAttendancesID { get; set; }

        [Required]
        public required Guid PatientID { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 4)]
        [AllowedValues(["Admit", "Continue care", "Discharge (Case completed)", "Discharg (For review)", "Discharg(Against medical advice", "Transfered (Internal)", "Transfered (External)", "Died", "Absconded"])]
        public required string Outcome { get; set; }

        [StringLength(150)]
        public string? Notes { get; set; }

        [Required]
        public required DateTime OutcomeDate { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(75, MinimumLength = 4)]
        public required string UserName { get; set; }

        public virtual PatientAttendance? PatientAttendance { get; set; }
    }

    public record AddPatientOutcomeDto(
        Guid PatientsAttendancesID,
        Guid PatientID,

        [StringLength(50, MinimumLength = 4)]
        [AllowedValues(["Admit", "Continue care", "Discharge (Case completed)", "Discharg (For review)", "Discharg(Against medical advice", "Transfered (Internal)", "Transfered (External)", "Died", "Absconded"])]
        string Outcome,
        [StringLength(150)]
        string? Notes);
    }
