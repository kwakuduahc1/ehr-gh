using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShimsServer.Models.Records
{
    public class PatientSchemes
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid PatientSchemesID { get; set; }

        [Required]
        public required Guid PatientsID { get; set; }
        
        [Required, StringLength(50)]
        public required string HospitalID { get; set; }

        [Required]
        public required Guid SchemesID { get; set; }

        [DefaultValue(true)]
        public bool IsActive { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 10)]
        public required string CardID { get; set; }

        [Required]
        public required DateTime ExpiryDate { get; set; }

        public DateTime LastUpdateDate { get; set; } = DateTime.UtcNow;

        [Required, StringLength(75, MinimumLength = 10)]
        public required string UserName { get; set; }
    }

    public record AddPatientSchemeDto(
    [Required]
    Guid PatientsID,

    [Required]
    Guid SchemesID,

    [Required, StringLength(30, MinimumLength = 10)]
    string CardID,

    [DefaultValue(true)]
    bool IsActive,

    [Required]
    DateTime ExpiryDate
);

    public record EditPatientSchemeDto(
        [Required]
    Guid PatientSchemesID, // needed to identify the record

        [Required]
    Guid PatientsID,

        [Required]
    Guid SchemesID,

        [DefaultValue(true)]
    bool Status,

        [Required, StringLength(30, MinimumLength = 10)]
    string CardID,

        [Required]
    DateTime ExpiryDate
    );
}
