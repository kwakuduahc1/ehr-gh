using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SHIMS.Models.Wards
{
    public class WardAdmissions
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid WardAdmissionsID { get; set; } = Guid.CreateVersion7();

        [Required]
        public required Guid PatientsAttendancesID { get; set; }

        [Required]
        public required Guid WardsID { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(75, MinimumLength = 5)]
        public required string UserName {  get; set; }

        [Required]
        public required DateTime DateAdmitted { get; set; } = DateTime.UtcNow;

        public DateTime? DateDischarged { get; set; }

        public virtual Records.PatientAttendance? PatientAttendance { get; set; }

        public virtual Wards? Wards { get; set; }
    }
}
