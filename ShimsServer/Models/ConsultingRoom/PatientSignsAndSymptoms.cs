using System.ComponentModel.DataAnnotations;

namespace ShimsServer.Models.ConsultingRoom
{
    public class PatientSignsAndSymptoms
    {
        [Key]
        public Guid PatientSignsID { get; set; }

        [Required]
        [StringLength(400, MinimumLength = 3)]
        public required string SignAndSymptoms { get; set; }

    }
}
