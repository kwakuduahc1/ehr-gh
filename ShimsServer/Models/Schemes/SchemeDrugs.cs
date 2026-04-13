using Microsoft.EntityFrameworkCore;
using ShimsServer.Models.Drugs;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShimsServer.Models.Schemes
{

    [Index(nameof(DrugCode), [nameof(DateSet), nameof(DosageForm), nameof(Strength), nameof(PricingUnit)])]
    public class SchemeDrugs
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid SchemeDrugsID { get; set; } = Guid.CreateVersion7();

        [Required]
        public required Guid SchemesID { get; set; }

        [Required]
        public required Guid DrugsID { get; set; }

        [Required, StringLength(50)]
        public required string DrugCode { get; set; }

        [Required, StringLength(100)]
        public required string DosageForm { get; set; }

        [Required,  StringLength(75)]
        public required string Strength { get; set; }

        [StringLength(50), Required]
        public required string PricingUnit { get; set; }

        [StringLength(50)]
        public string? PrescriptionLevel { get; set; }


        [Required]
        [Range(0.5D, double.MaxValue)]
        public required decimal Price { get; set; }

        [Required]
        public required DateTime DateSet { get; set; }

        [DefaultValue(false)]
        public bool IsActive { get; set; } = true;

        [Required]
        [StringLength(75, MinimumLength = 10)]
        public required string UserName { get; set; }

        public virtual Drugs.Drugs? Drugs { get; set; }

        public virtual Schemes? Schemes { get; set; }

        public virtual ICollection<DrugsRequests>? DrugsRequests { get; set; }
    }
}
