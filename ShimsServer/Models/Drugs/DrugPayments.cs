using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShimsServer.Models.Drugs
{
    public class DrugPayments
    {
        [Key, Required]
        [ForeignKey(nameof(DispensingCalculations))]
        public required Guid DispensingCaculationsID { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 8)]
        public required string Receipt { get; set; }

        [Required]
        public byte QuantityPaid { get; set; }

        public DateTime? DatePaid { get; set; }

        [Required]
        public decimal Amount{ get; set; }

        public Guid? PaymentTypesID { get; set; }

        [StringLength(75, MinimumLength = 10)]
        [Required]
        public required string UserName { get; set; }

        public virtual DispensingCalculations? DispensingCalculations { get; set; }

        public virtual Dispensing? Dispensing { get; set; }
    }
}
