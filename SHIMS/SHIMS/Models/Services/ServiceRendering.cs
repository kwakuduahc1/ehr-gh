using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SHIMS.Models.Services
{
    public class ServiceRendering
    {
        [Key, Required]
        [ForeignKey(nameof(ServicePayment))]
        public required Guid ServicePaymentID { get; set; }

        [Required]
        public required Guid PatientsAttendancesID { get; set; }

        [Required]
        public required DateTime DateServed { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(75, MinimumLength = 10)]
        public required string UserName { get; set; }

        [Required]
        [StringLength(500)]
        public required string Report { get; set; }

        public virtual ServicePayment? ServicePayment { get; set; }
    }
}
