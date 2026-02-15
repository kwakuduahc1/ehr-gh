using SHIMS.Models.Records;
using SHIMS.Models.Schemes;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SHIMS.Models.Drugs
{
    public class DrugsRequests
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid DrugsRequestsID { get; set; } = Guid.CreateVersion7();

        [Required]
        public required Guid PatientsAttendancesID { get; set; }

        [Required]
        public required Guid SchemeDrugsID { get; set; }    

        [Required]
        [Range(1, 6, ErrorMessage = "Frequency must be between {0} and {1} daily")]
        public byte Frequency { get; set; }

        [Range(1, 200), Required(ErrorMessage = "Kindly Indicate the {0} for this drug")]
        [Display(Name = "Days")]
        public byte Days { get; set; }

        public DateTime DateRequested { get; set; } = DateTime.UtcNow;

        [Range(0, 200)]
        public byte QuantityRequested { get; set; }

        [StringLength(75, MinimumLength = 10)]
        [Required]
        public required string UserName { get; set; }

        public bool IsPaid { get; set; } = false;

        public bool IsDispensed { get; set; } = false;

        public virtual SchemDrugs? SchemeDrugs { get; set; }

        public virtual PatientAttendance? PatientsAttendances { get; set; }

        public virtual DispensingCalculations? DispensingCalculations { get; set; }
    }
}
