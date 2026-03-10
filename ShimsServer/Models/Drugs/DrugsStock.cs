using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShimsServer.Models.Drugs
{
    public class DrugsStock
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid DrugsStockID { get; set; } = Guid.CreateVersion7(DateTimeOffset.UtcNow);

        [Required]
        public required short Quantity { get; set; }

        public DateTime TranDate { get; set; } = DateTime.UtcNow;

        [Required]
        public required Guid DrugsID { get; set; }

        public virtual Drugs? Drugs { get; set; }
    }
}
