using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SHIMS.Models.Labs
{
    public class PatientLabResults
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PatientLabResultsID { get; set; }

        public required DateTime DateRequested { get; set; } = DateTime.UtcNow;

        [Required]
        public required Guid PatientID { get; set; }
        
        [Required]
        public required int LabParametersID { get; set; }

        [Required, StringLength(50, MinimumLength = 1)]
        public required string Result { get; set; }

        [StringLength(100, MinimumLength = 2)]
        public string Notes { get; set; } = string.Empty;

        [Required]
        public required string Requester { get; set; }

        public DateTime? DateServed { get; set; }

        public string? ServerName { get; set; }

        public DateTime? DatePaid { get; set; }

        public decimal? AmountPaid { get; set; } = null;

        public string? PaymentReceiver { get; set; } = string.Empty;

        public virtual LabParameters? LabParameter { get; set; }
    }
}
