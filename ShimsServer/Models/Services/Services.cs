using ShimsServer.Models.Schemes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShimsServer.Models.Services
{
    public class Services
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid ServicesID { get; set; }

        [StringLength(200)]
        [Required]
        public required string Service { get; set; }

        [DefaultValue(false)]
        public bool IsDeleted { get; set; } = false;

        [Required]
        [StringLength(200)]
        public required string ServiceGroup { get; set; }

        public virtual ICollection<SchemeServices>? SchemeServices{ get; set; }
    }
}
