using ShimsServer.Models.Records;
using ShimsServer.Models.Schemes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShimsServer.Models.Services
{
    public class ServiceRequest
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ServiceRequestID { get; set; } = Guid.CreateVersion7();

        [Required]
        public required Guid PatientsAttendancesID { get; set; }

        [Required]
        public required Guid SchemeServicesID { get; set; }

        [Required]
        [Range(1, 3)]
        public byte Frequency { get; set; }

        [StringLength(75, MinimumLength = 10)]
        [Required]
        public required string UserName { get; set; }

        [Required]
        public required DateTime DateRequested { get; set; } = DateTime.UtcNow;

        public virtual PatientAttendance? PatientAttendance { get; set; }

        public virtual SchemeServices? SchemeServices { get; set; }

        public virtual ServicePayment? ServicePayment { get; set; }
    }
}
