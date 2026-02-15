using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SHIMS.Models.Labs
{
    public class LabResults
    {
        [Key, Required]
        [ForeignKey(nameof(LabPayment))]
        public required Guid LabPaymentID { get; set; }

        [Required]
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

        public virtual LabPayment? LabPayment { get; set; }

        public virtual LabParameters? LabParameter { get; set; }
    }
}
