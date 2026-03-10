using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShimsServer.Models.Drugs
{
    public class Drugs
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid DrugsID { get; set; } = Guid.CreateVersion7(DateTimeOffset.UtcNow);

        [StringLength(150)]
        [Required(AllowEmptyStrings = false)]
        public required string Drug { get; set; }

        public string? Tags { get; set; }

        [StringLength(100)]
        public string? Description { get; set; }

        public DateTime DateAdded { get; set; }

        public virtual ICollection<DrugsRequests>? PatientDrugs { get; set; }

        public virtual ICollection<DrugsStock>? DrugsStocks { get; set; }
    }
}
