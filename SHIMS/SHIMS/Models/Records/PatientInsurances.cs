using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SHIMS.Models.Records
{
    public class PatientInsurances
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid PatientInsurancesID { get; set; }

        [Required]
        public required Guid PatientsID { get; set; }

        [Required]
        public required Guid InsurancesID { get; set; }

        [DefaultValue(true)]
        public bool Status { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 10)]
        public required string CardID { get; set; }

        [Required]
        public required DateTime ExpiryDate { get; set; }

        public DateTime LastUpdateDate { get; set; } = DateTime.UtcNow;

        [Required, StringLength(75, MinimumLength = 10)]
        public required string UserName { get; set; }
    }

    public record AddPatientInsuranceDto(
    [Required]
    Guid PatientsID,

    [Required]
    Guid InsurancesID,

    [DefaultValue(true)]
    bool Status,

    [Required, StringLength(30, MinimumLength = 10)]
    string CardID,

    [Required]
    DateTime ExpiryDate
);

    public record EditPatientInsuranceDto(
        [Required]
    Guid PatientInsurancesID, // needed to identify the record

        [Required]
    Guid PatientsID,

        [Required]
    Guid InsurancesID,

        [DefaultValue(true)]
    bool Status,

        [Required, StringLength(30, MinimumLength = 10)]
    string CardID,

        [Required]
    DateTime ExpiryDate
    );
}
