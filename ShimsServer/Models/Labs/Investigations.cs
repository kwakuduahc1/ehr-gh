using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShimsServer.Models.Labs
{

    public class Investigations
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid InvestigationsID { get; set; }
        
        [Required, StringLength(150, MinimumLength = 3)]
        public required string Investigation { get; set; }

        [Required, StringLength(50, MinimumLength = 3)]
        public required string Category { get; set; }

        [Required, StringLength(20, MinimumLength = 3)]
        public required string InvestigationType { get; set; }

        [Required]
        [Column(TypeName = "varchar(5)[]")]
        public required string[] Levels { get; set; }

        [StringLength(500, MinimumLength = 2)]
        public string? LabDescription { get; set; }

        public virtual ICollection<InvestigationParameters>? LabParameters { get; set; }

        public virtual ICollection<Schemes.SchemeInvestigations>? SchemeLabs { get; set; }
    }
}
