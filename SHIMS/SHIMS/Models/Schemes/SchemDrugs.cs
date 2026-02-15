using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SHIMS.Models.Schemes
{
    public class SchemDrugs
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]

        public required Guid SchemesID { get; set; }

        public required Guid DrugsID { get; set; }

        [Required]
        [Range(0.5D, double.MaxValue)]
        public required decimal Price { get; set; }

        [Required]
        public required DateTime DateSet { get; set; }

        [Required]
        [StringLength(75, MinimumLength = 10)]
        public required string UserName { get; set; }

        public virtual Drugs.Drugs? Drugs { get; set; }

        public virtual Schemes? Schemes { get; set; }
    }
}
