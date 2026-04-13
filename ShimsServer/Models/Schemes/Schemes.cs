using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShimsServer.Models.Schemes
{
    public class Schemes
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid SchemesID { get; set; }

        [Required, StringLength(50, MinimumLength = 3)]
        public required string SchemeName { get; set; }

        [Required]
        [StringLength(30)]
        [AllowedValues(["Full", "Relative", "Fixed"])]
        public required string Coverage { get; set; }

        [DefaultValue(100)]
        [Range(0, double.MaxValue)]
        public decimal MaxPayable {  get; set; }

        [DefaultValue(100)]
        [Range(1,100.0)]
        public required decimal Recovery { get; set; }

        [DefaultValue(true)]
        public bool IsActive { get; set; } = true;
    }
}
