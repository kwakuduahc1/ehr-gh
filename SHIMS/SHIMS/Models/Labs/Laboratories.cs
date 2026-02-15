using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SHIMS.Models.Labs
{
    public class LabGroups
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid LabGroupsID { get; set; }
        
        [Required, StringLength(50, MinimumLength = 3)]
        public required string LabGroup { get; set; }

        [StringLength(100, MinimumLength = 2)]
        public string? LabDescription { get; set; }

        public virtual ICollection<LabParameters>? LabParameters { get; set; }

        public virtual ICollection<Schemes.SchemeLabs>? SchemeLabs { get; set; }
    }
}
