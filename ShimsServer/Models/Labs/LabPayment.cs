using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShimsServer.Models.Labs
{
    public class LabPayment
    {
        [Key, Required]
        [ForeignKey(nameof(LabRequests))]
        public required Guid LabRequestsID { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 8)]
        public required string Receipt { get; set; }

        public DateTime? DatePaid { get; set; }

        [Required]
        [Range(0.0, double.MaxValue)]
        [DataType(DataType.Currency)]
        public decimal Amount { get; set; } = 0;

        public Guid? PaymentTypesID { get; set; }

        [StringLength(75, MinimumLength = 10)]
        public string? PaymentReceiver { get; set; }

        [StringLength(75, MinimumLength = 10)]
        [Required]
        public required string UserName { get; set; }

        public virtual LabRequests? LabRequests { get; set; }

        public virtual LabResults? LabResults { get; set; }
    }
}
