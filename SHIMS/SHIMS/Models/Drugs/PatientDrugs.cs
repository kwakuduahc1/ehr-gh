using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SHIMS.Models.Drugs
{
    public class PatientDrugs
    {
        [Key]
        public Guid ID { get; set; }

        [Required]
        public Guid PatientsAttendancesID { get; set; }

        [Required(ErrorMessage = "{0} is required")]
        public Guid SchemeDrugsID { get; set; }

        [Range(1, 6, ErrorMessage = "Frequency must be between {0} and {1} daily")]
        public byte Frequency { get; set; }

        [Range(1, 200), Required(ErrorMessage = "Kindly Indicate the {0} for this drug")]
        [Display(Name = "Days")]
        public byte Days { get; set; }

        [Required]
        [StringLength(20, MinimumLength = 8)]
        public required string Receipt { get; set; }

        [Range(0, 200)]
        public byte QuantityRequested { get; set; }

        [Range(0, 200)]
        public byte QuantityIssued { get; set; }

        public DateTime? DatePaid { get; set; }

        [DefaultValue(false)]
        public bool HasPaid { get; set; }

        [Display(Name = "Receiving Officer")]
        [StringLength(75, MinimumLength = 10)]
        public string? ReceivingOfficer { get; set; }

        [DefaultValue(false)]
        public bool IsServed { get; set; }

        public DateTime? DateServed { get; set; }

        [Display(Name = "Serving Officer")]
        [StringLength(75, MinimumLength = 10)]
        public string? ServingOfficer { get; set; }

        public DateTime DateRequested { get; set; }

        [DataType(DataType.Currency)]
        public decimal AmountPaid { get; set; } = 0;

        public Guid? PaymentTypesID { get; set; }

        [Required]
        [Range(0.0, double.MaxValue)]
        [DataType(DataType.Currency)]
        public decimal Cost { get; set; }

        [Display(Name = "Requested by?"), MaxLength(30)]
        public string? RequestingOficcer { get; set; }

        public bool Deleted { get; set; } = false;

        public virtual Patients? Patients { get; set; }

        public virtual Drugs? Drugs { get; set; }
    }
}
