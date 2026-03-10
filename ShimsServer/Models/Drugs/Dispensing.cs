using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShimsServer.Models.Drugs
{
    public class Dispensing
    {
        [Key, Required]
        [ForeignKey(nameof(DrugPayments))]
        public required Guid DrugPaymentsID { get; set; }

        [Required]

        public DateTime DateDispensed { get; set; } = DateTime.UtcNow;

        [Required]
        [Range(0, 100)]
        public byte QuantityDispensed { get; set; }

        [StringLength(75, MinimumLength = 10)]
        [Required]
        public required string UserName { get; set; }

        public virtual DrugPayments? DrugPayments { get; set; }
    }
}
