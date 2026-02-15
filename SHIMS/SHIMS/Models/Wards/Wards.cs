using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SHIMS.Models.Wards
{
    public class Wards
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid WardsID { get; set; }

        [Required, StringLength(50, MinimumLength = 5)]
        public required string WardName { get; set; }

        [Required, StringLength(10, MinimumLength = 4)]
        public required string WardTags { get; set; }

        [Required]
        [Range(5, 50)]
        public short Capacity { get; set; }
    }
}
