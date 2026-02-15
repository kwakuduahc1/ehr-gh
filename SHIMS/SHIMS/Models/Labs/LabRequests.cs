using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SHIMS.Models.Records;
using SHIMS.Models.Schemes;

namespace SHIMS.Models.Labs
{
    public class LabRequests
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid LabRequestsID { get; set; } = Guid.CreateVersion7();

        [Required]
        public required Guid PatientsAttendancesID { get; set; }

        [Required]
        public required Guid SchemeLabsID { get; set; }

        [Required]
        public required DateTime DateRequested { get; set; } = DateTime.UtcNow;

        [StringLength(75, MinimumLength = 10)]
        [Required]
        public required string UserName { get; set; }

        public virtual PatientAttendance? PatientAttendance { get; set; }

        public virtual SchemeLabs? SchemeLabs { get; set; }

        public virtual LabPayment? LabPayment { get; set; }
    }
}
