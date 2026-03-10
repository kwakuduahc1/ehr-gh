using ShimsServer.Models.Records;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShimsServer.Models.ConsultingRoom
{
    public class PatientDiagnosis
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid PatientDiagnosisID { get; set; } = Guid.CreateVersion7();

        [Required]
        public required Guid PatientsAttendancesID { get; set; }

        [Required]
        public Guid DiagnosisID { get; set; }

        public Guid[]? SecondaryDiagnoses { get; set; }

        [StringLength(200, ErrorMessage = "{0} should be less than {1} characters")]
        public required string Description { get; set; }

        [Display(Name = "Date added")]
        public DateTime DateAdded { get; set; }

        [StringLength(75, MinimumLength =10)]
        public required string UserName { get; set; }

        public virtual PatientAttendance? PatientAttendance { get; set; }

        public virtual Diagnoses? Diagnoses { get; set; }
    }
}
