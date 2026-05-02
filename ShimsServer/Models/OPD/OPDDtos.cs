using ShimsServer.Repositories;
using System.ComponentModel.DataAnnotations;

namespace ShimsServer.Models.OPD
{
    public record VitalsummaryDto(
       IEnumerable<VitalsDTO> Vitals,
        LitePatientDto Patient);
    // DTOs for OPD (Out-Patient Department) Vitals

    /// <summary>
    /// Data transfer object for vital signs information
    /// </summary>
    public record VitalsDTO(
        Guid VitalsID,
        Guid PatientsAttendancesID,
        DateTime DateSeen,
        double Temperature,
        double Weight,
        double? Pulse,
        double? Systol,
        double? Diastol,
        double? Respiration,
        double? SPO2,
        string Complaints,
        string? Notes,
        string UserName
        );

    /// <summary>
    /// Data transfer object for creating vital signs records
    /// </summary>
    public record AddVitalsDto(
        [Required] Guid PatientsAttendancesID,
        [Required, Range(36, 45.0, ErrorMessage = "Temperature must be between 36 and 45 degrees Celsius")] double Temperature,
        [Required, Range(1.8, 250, ErrorMessage = "Weight must be between 1.8 and 250 kg")] double Weight,
        [Range(20, 250, ErrorMessage = "Pulse must be between 20 and 250 bpm")] double? Pulse,
        [Range(20, 250, ErrorMessage = "Systolic pressure must be between 20 and 250 mmHg")] double? Systol,
        [Range(20, 250, ErrorMessage = "Diastolic pressure must be between 20 and 250 mmHg")] double? Diastol,
        [Range(12, 60, ErrorMessage = "Respiration rate must be between 12 and 60 breaths/min")] double? Respiration,
        [Range(50.0, 110.0, ErrorMessage = "SPO2 must be between 50 and 110%")] double? SPO2,
        [Required, StringLength(200, MinimumLength = 3, ErrorMessage = "Complaints must be between 3 and 200 characters")] string Complaints,
        [StringLength(200, MinimumLength = 5, ErrorMessage = "Notes must be between 5 and 200 characters")] string? Notes);
}
