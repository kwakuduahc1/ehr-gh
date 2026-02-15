using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SHIMS.Models.Labs
{
    public class LabParameters
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LabParametersID { get; set; }
       
        [Required, StringLength(50, MinimumLength = 3)]
        public required string LabParameter { get; set; }

        [Required, Range(0, 100)]
        public short Order { get; set; }

        [Required]
        public required int LabGroupsID { get; set; }

        public virtual LabGroups? LabGroup { get; set; }
    }
}
