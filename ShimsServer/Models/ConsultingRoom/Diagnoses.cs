using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShimsServer.Models.ConsultingRoom
{
    [Index("ICD", ["GDRG", "SNOMED"], IsUnique = true)]
    public class Diagnoses
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid DiagnosisID { get; set; } = Guid.CreateVersion7();

        [Required, StringLength(15)]
        public required string ICD { get; set; }

        [StringLength(30, MinimumLength = 3)]
        public string? GDRG { get; set; }

        [StringLength(50, MinimumLength = 3)]
        public string? SNOMED { get; set; }
    }
}
