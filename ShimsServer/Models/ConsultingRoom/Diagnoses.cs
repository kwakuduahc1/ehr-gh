using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShimsServer.Models.ConsultingRoom
{

    [Index(nameof(DiagnosisName), [nameof(Category), nameof(SubCategory)])]
    public class Diagnoses
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid DiagnosesID { get; set; } = Guid.CreateVersion7();

        [Required, StringLength(255)]
        public required string DiagnosisName { get; set; }

        [StringLength(100)]
        public string? Category { get; set; }

        [Required]
        [Column(TypeName = "varchar(5)[]")]
        public string[]? Levels { get; set; }


        [StringLength(100)]
        public string? SubCategory { get; set; }


        [StringLength(150)]
        public string? Description { get; set; }

        [DefaultValue(true)]
        public bool IsActive { get; set; } = true;
        public virtual ICollection<SchemeDiagnoses>? SchemeDiagnoses { get; set; }
    }

    public class SchemeDiagnoses
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid SchemeDiagnosesID { get; set; } = Guid.CreateVersion7();

        [Required]
        [ForeignKey(nameof(Schemes))]
        public required Guid SchemesID { get; set; }

        [Required]
        [ForeignKey(nameof(Diagnoses))]
        public required Guid DiagnosesID { get; set; }

        [Required, StringLength(255)]
        public required string Variation { get; set; }  // e.g., "Malaria - simple", "Malaria - cerebral"

        [StringLength(10)]
        [Required]
        public required string ICDCode { get; set; }

        [Required]
        [StringLength(20)]
        public required string ICDVersion { get; set; }

        [StringLength(30)]
        public string? Snomed { get; set; }


        [Required, StringLength(20)]
        public required string GDRG { get; set; }

        [Required]
        [Range(0.5D, double.MaxValue)]
        public required decimal Tariff { get; set; }

        [DefaultValue(true)]
        public bool IsActive { get; set; } = true;

        [Required]
        public required DateTime DateSet { get; set; }

        [Required]
        [StringLength(75, MinimumLength = 10)]
        public required string UserName { get; set; }

        public virtual Schemes.Schemes? Schemes { get; set; }

        public virtual Diagnoses? Diagnoses { get; set; }
    }
}
