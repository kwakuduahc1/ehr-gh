using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SHIMS.Models.Wards
{
    public class WardAdmissions
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid WardAdmissionsID { get; set; }

        [Required]
        public Guid PatientsID { get; set; }

        [Required]
        public Guid WardsID { get; set; }

        [Required(AllowEmptyStrings = false)]
        [StringLength(75, MinimumLength = 5)]
        public required string Physician {  get; set; }

        [Required]
        public required DateTime DateAdmitted { get; set; } = DateTime.UtcNow;

        public virtual Records.PatientAttendance? PatientAttendance { get; set; }

        public virtual Wards? Wards { get; set; }
    }
}
