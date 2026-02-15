using System.ComponentModel.DataAnnotations;

namespace SHIMS.Models.ConsultingRoom
{
    public class PatientSignsAndSymptoms
    {
        [Key]
        public Guid PatientSignsID { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 3)]
        public required string SignAndSymptoms { get; set; }

    }
}
