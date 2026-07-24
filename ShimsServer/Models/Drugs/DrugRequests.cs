using ShimsServer.Models.Records;
using ShimsServer.Models.Schemes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShimsServer.Models.Drugs
{
    public class DrugsRequests
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid DrugsRequestsID { get; set; } = Guid.CreateVersion7();

        [Required]
        [ForeignKey(nameof(PatientAttendance))]
        public required Guid PatientAttendancesID { get; set; }

        [Required]
        public DateTime RequestDate { get; set; } = DateTime.UtcNow;

        [Required]
        [StringLength(30, MinimumLength = 5)]
        public required string Physician { get; set; }

        [DefaultValue(false)]
        public bool IsPaid { get; set; } = false;

        [Required]
        public DateTime? DatePaid { get; set; }

        [StringLength(20, MinimumLength = 8)]
        public string PaymentReceipt { get; set; } = string.Empty;

        [StringLength(30, MinimumLength = 5)]
        public string? PaymentUser { get; set; }

        [DefaultValue(false)]
        public bool IsDispensed { get; set; } = false;

        public DateTime? DateDispensed { get; set; }

        [StringLength(30, MinimumLength = 5)]
        public string? DispensingUser { get; set; }

        public virtual PatientAttendance? PatientAttendance { get; set; }

        public virtual ICollection<DrugsRequestDetails>? DrugsRequestDetails { get; set; }


    }
    public class DrugsRequestDetails
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid DrugsRequestDetailsID { get; set; } = Guid.CreateVersion7();

        [Required]
        public required Guid DrugsRequestsID { get; set; }

        [Required]
        [ForeignKey(nameof(SchemeDrugs))]
        public required Guid SchemeDrugsID { get; set; }

        [Required]
        [Range(0, 6, ErrorMessage = "Frequency must be between {0} and {1} daily")]
        public byte Frequency { get; set; }

        [Range(1, 200), Required(ErrorMessage = "Kindly Indicate the {0} for this drug")]
        [Display(Name = "Days")]
        public byte Days { get; set; }

        [DefaultValue(false)]
        public bool IsQuantitySet { get; set; } = false;

        [Range(0, 200)]
        public byte? QuantityRequested { get; set; }

        public DateTime? DateRequested { get; set; }

        [DefaultValue(false)]
        public bool IsDispensed { get; set; } = false;

        [Range(0, 200)]
        public byte? QuantityDispensed { get; set; }

        public DateTime? DateDispensed { get; set; }

        [StringLength(150, MinimumLength = 10)]
        public string? Notes { get; set; }

        [Required]
        public required string UserName { get; set; }

        public virtual SchemeDrugs? SchemeDrugs { get; set; }

        public virtual DrugsRequests? DrugsRequests { get; set; }
    }
}
