using SHIMS.Models.Labs;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SHIMS.Models.Schemes
{
    public class SchemeLabs
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid SchemeLabsID { get; set; } = Guid.CreateVersion7();

        [Required]
        public required Guid LabsGroupID { get; set; }

        [Required]
        public required Guid SchemesID { get; set; }

        [Required]
        [Range(0.5D, double.MaxValue)]
        public required decimal Price { get; set; }

        [Required]
        public required DateTime DateSet { get; set; }

        [Required]
        [StringLength(75, MinimumLength = 10)]
        public required string UserName { get; set; }

        public virtual Schemes? Schemes { get; set; }

        public virtual LabGroups? LabGroups { get; set; }

        public virtual ICollection<LabRequests>? LabRequests { get; set; }
    }
}
