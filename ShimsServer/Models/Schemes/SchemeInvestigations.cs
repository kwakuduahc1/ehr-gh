using ShimsServer.Models.Labs;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShimsServer.Models.Schemes
{
    //[Index([nameof(GDRG), nameof(SchemesID), nameof(InvestigationsID)])]
    public class SchemeInvestigations
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid SchemeInvestigationsID { get; set; } = Guid.CreateVersion7();

        [Required]
        [ForeignKey(nameof(Investigations))]
        public required Guid InvestigationsID { get; set; }

        [Required]
        [ForeignKey(nameof(Schemes))]
        public required Guid SchemesID { get; set; }

        [Required, StringLength(20)]
        public required string GDRG { get; set; }

        [Required]
        [Range(0.5D, double.MaxValue)]
        public required decimal Price { get; set; }

        [DefaultValue(true)]
        public bool IsActive { get; set; } = true;

        [Required]
        public required DateTime DateSet { get; set; }

        [Required]
        [StringLength(75, MinimumLength = 10)]
        public required string UserName { get; set; }

        public virtual Schemes? Schemes { get; set; }

        public virtual Investigations? Investigations { get; set; }

        public virtual ICollection<InvestigationsRequests>? InvestigationsRequests { get; set; }
    }
}
