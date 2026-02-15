using SHIMS.Models.Services;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SHIMS.Models.Schemes
{
    public class SchemeServices
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public required Guid SchemeServicesID { get; set; } = Guid.CreateVersion7();

        [Required]
        public required Guid SchemesID { get; set; }

        [Required]
        public required Guid ServicesID { get; set; }

        [Required]
        [Range(0.5D, double.MaxValue)]
        public required decimal Price { get; set; }

        [Required]
        public required DateTime DateSet { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(75, MinimumLength = 10)]
        public required string UserName { get; set; }

        public virtual Schemes? Schemes { get; set; }

        public virtual Services.Services? Services { get; set; }

        public virtual ICollection<ServiceRequest>? ServiceRequests { get; set; }
    }
}
