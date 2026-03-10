using SHIMS.Models.Records;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SHIMS.Models.OPD
{
    public class Vitals
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid VitalsID { get; set; } = Guid.CreateVersion7(DateTimeOffset.UtcNow);

        [Required]
        public Guid PatientsAttendancesID { get; set; }

        public required DateTime DateSeen { get; set; } = DateTime.UtcNow;

        [Required, Range(36, 45.0)]
        public required double Temperature { get; set; }

        [Required, Range(1.8, 250)]
        public required double Weight { get; set; }

        [Range(20, 250)]
        public double Pulse { get; set; }

        [Range(20, 250)]
        public double Systol { get; set; }

        [Range(20, 250)]
        public double Diastol { get; set; }

        [Range(12, 60)]
        public double Respiration { get; set; }

        [Range(50.0, 110)]
        public double? SPO2 { get; set; }

        [Required]
        [StringLength(200, MinimumLength = 3)]
        public required string Complaints { get; set; }

        [StringLength(200, MinimumLength = 5)]
        [Required]
        public string? Notes { get; set; }

        [Required, StringLength(75, MinimumLength = 10)]
        public required string UserName { get; set; }

        public virtual PatientAttendance? PatientAttendance { get; set; }
    }

    public record AddVitalsDto(
        Guid PatientsAttendancesID,


    [Range(36, 45.0)]
    double Temperature,

    [Range(1.8, 250)]
    double Weight,

    [Range(20, 250)]
    double? Pulse,

    [Range(20, 250)]
    double? Systol,

    [Range(20, 250)]
    double? Diastol,

    [Range(12, 60)]
    double? Respiration,

    [Range(50, 110.0)]
    double? SPO2,

    [StringLength(200, MinimumLength = 3)]
    string Complaints,

    [StringLength(200, MinimumLength = 5)]
    string? Notes
);
}
