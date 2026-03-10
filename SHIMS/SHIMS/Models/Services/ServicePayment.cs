using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SHIMS.Models.Services
{
    public class ServicePayment
    {
        [Key, Required]
        [ForeignKey(nameof(ServiceRequest))]
        public required Guid ServiceRequestID { get; set; }

        [Required]
        public required Guid PatientsAttendancesID { get; set; }

        [Required(ErrorMessage = "Indicate the cost of the service")]
        [Range(0, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required, AllowedValues(["Cash", "Mobile Money", "Insurance"])]
        [StringLength(20)]
        public required string PaymentMethod { get; set; }

        public DateTime? DatePaid { get; set; }

        [StringLength(20, MinimumLength = 8, ErrorMessage = "{0} should be between {1} and {2} characters")]
        public string? Receipt { get; set; }

        [StringLength(75, MinimumLength = 10)]
        [Required]
        public required string UserName { get; set; }

        public virtual ServiceRequest? ServiceRequest { get; set; }

        public virtual ServiceRendering? ServiceRendering { get; set; }
    }
}
