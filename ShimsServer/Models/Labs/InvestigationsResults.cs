using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShimsServer.Models.Labs
{
    public class InvestigationsResults
    {
        [Key, Required]
        [ForeignKey(nameof(InvestigationsPayment))]
        public required Guid InvestigationsPaymentID { get; set; }

        [Required]
        [ForeignKey(nameof(InvestigationParameters))]
        public required Guid LabParametersID { get; set; }

        [Required, StringLength(50, MinimumLength = 1)]
        public required string Result { get; set; }

        [StringLength(500, MinimumLength = 2)]
        public string? Notes { get; set; }

        [Required]
        public required DateTime DateTested { get; set; } = DateTime.UtcNow;

        public string? ServerName { get; set; }

        [StringLength(75, MinimumLength = 10)]
        [Required]
        public required string UserName { get; set; }

        public virtual InvestigationsPayment? InvestigationsPayment { get; set; }

        public virtual InvestigationParameters? InvestigationParameters { get; set; }
    }
}
