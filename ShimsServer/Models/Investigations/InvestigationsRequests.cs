using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ShimsServer.Models.Records;
using ShimsServer.Models.Schemes;

namespace ShimsServer.Models.Investigations
{
    public class InvestigationsRequests
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid InvestigationsRequestsID { get; set; } = Guid.CreateVersion7();

        [Required]
        [ForeignKey(nameof(PatientAttendance))]
        public required Guid PatientAttendancesID { get; set; }

        [Required, ForeignKey(nameof(Investigations))]
        public required Guid InvestigationsID { get; set; }


        [Required]
        public required DateTime DateRequested { get; set; } = DateTime.UtcNow;

        [StringLength(75, MinimumLength = 10)]
        [Required]
        public required string UserName { get; set; }

        public virtual PatientAttendance? PatientAttendance { get; set; }

        public virtual Investigations? Investigations { get; set; }

        public virtual InvestigationsPayment? LabPayment { get; set; }
    }
}
