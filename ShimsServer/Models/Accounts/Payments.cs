using ShimsServer.Models.Records;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ShimsServer.Models.Accounts
{
    public class Payments
    {
        [Key]
        public Guid PaymentsID { get; set; } = Guid.CreateVersion7();

        [Required]
        public Guid PatientsAttendancesID { get; set; }

        [DefaultValue(0)]
        public decimal Drugs { get; set; }

        [Required]
        public required Guid[] DrugsList { get; set; }

        [DefaultValue(0)]
        public decimal Labs { get; set; }

        [Required]
        public required Guid[] LabsList { get; set; }

        [DefaultValue(0)]
        public decimal Services { get; set; }

        [Required]
        public required Guid[] ServicesList { get; set; }

        [DefaultValue(0)]
        public decimal Registration { get; set; }

        [DefaultValue(0)]
        public decimal Attendance { get; set; }

        public DateTime PaymentDate { get; set; }

        [StringLength(75, MinimumLength = 10)]
        [Required]
        public required string UserName { get; set; }

        [DefaultValue(0)]
        [Required]
        public decimal Cash{ get; set; }

        [DefaultValue(0)]
        public decimal MobileMoney { get; set; }

        public virtual PatientAttendance? PatientAttendance { get; set; }
    }
}
