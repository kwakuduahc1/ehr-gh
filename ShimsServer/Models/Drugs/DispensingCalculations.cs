using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShimsServer.Models.Drugs
{
    public class DispensingCalculations
    {
        [Key, Required]
        [ForeignKey(nameof(DrugsRequests))]
        public required Guid DrugsRequestsID { get; set; }

        [Required]
        [Range(0, 100)]
        public byte Quantity { get; set; }

        [Required]
        public DateTime DateDone{ get; set; } = DateTime.UtcNow;

        [StringLength(75, MinimumLength = 10)]
        [Required]
        public required string UserName { get; set; }

        [StringLength(150, MinimumLength = 10)]
        public string? Notes { get; set; }

        public virtual DrugsRequests? DrugsRequests { get; set; }

        public virtual DrugPayments? DrugPayments { get; set; }
    }
}
