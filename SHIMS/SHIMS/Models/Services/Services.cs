using SHIMS.Models.Schemes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SHIMS.Models.Services
{
    public class Services
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ServicesID { get; set; }

        [StringLength(100)]
        [Required]
        public required string Service { get; set; }

        [Required]
        [StringLength(50)]
        public required string ServiceGroup { get; set; }

        public virtual ICollection<SchemeServices>? SchemeServices{ get; set; }
    }
}
