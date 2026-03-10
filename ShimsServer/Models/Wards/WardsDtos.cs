using System.ComponentModel.DataAnnotations;

namespace ShimsServer.Models.Wards
{
    // DTOs for Wards Management

    /// <summary>
    /// Data transfer object for ward information
    /// </summary>
    public record WardsDTO(
        Guid WardsID,
        string Ward,
        string WardTags,
        short Capacity);

    /// <summary>
    /// Data transfer object for creating a new ward
    /// </summary>
    public record AddWardsDto(
        [Required, StringLength(50, MinimumLength = 5, ErrorMessage = "Ward name must be between 5 and 50 characters")] string Ward,
        [Required, StringLength(10, MinimumLength = 4, ErrorMessage = "Ward tag must be between 4 and 10 characters")] string WardTags,
        [Required, Range(5, 50, ErrorMessage = "Ward capacity must be between 5 and 50 beds")] short Capacity);

    /// <summary>
    /// Data transfer object for updating ward information
    /// </summary>
    public record UpdateWardsDto(
        Guid WardsID,
        [Required, StringLength(50, MinimumLength = 5, ErrorMessage = "Ward name must be between 5 and 50 characters")] string Ward,
        [Required, StringLength(10, MinimumLength = 4, ErrorMessage = "Ward tag must be between 4 and 10 characters")] string WardTags,
        [Required, Range(5, 50, ErrorMessage = "Ward capacity must be between 5 and 50 beds")] short Capacity);

    /// <summary>
    /// Data transfer object for ward with current occupancy
    /// </summary>
    public record WardOccupancyDto(
        Guid WardsID,
        string Ward,
        string WardTags,
        short Capacity,
        int CurrentOccupancy,
        int AvailableBeds);

    // DTOs for Ward Admissions

    /// <summary>
    /// Data transfer object for ward admission information
    /// </summary>
    public record WardAdmissionDTO(
        Guid WardAdmissionsID,
        Guid PatientsAttendancesID,
        Guid WardsID,
        string PatientName,
        string WardName,
        DateTime DateAdmitted,
        DateTime? DateDischarged,
        string UserName);

    /// <summary>
    /// Data transfer object for creating ward admission
    /// </summary>
    public record AddWardAdmissionDto(
        [Required] Guid PatientsAttendancesID,
        [Required] Guid WardsID);

    /// <summary>
    /// Data transfer object for updating ward admission (discharge)
    /// </summary>
    public record UpdateWardAdmissionDto(
        Guid WardAdmissionsID,
        [Required] Guid PatientsAttendancesID,
        [Required] Guid WardsID);

    /// <summary>
    /// Data transfer object for discharging a patient from ward
    /// </summary>
    public record DischargePatientDto(
        Guid WardAdmissionsID);

    /// <summary>
    /// Data transfer object for ward admission summary with patient and ward details
    /// </summary>
    public record WardAdmissionSummaryDto(
        Guid WardAdmissionsID,
        string PatientName,
        string OPDNumber,
        string WardName,
        string WardTag,
        DateTime DateAdmitted,
        DateTime? DateDischarged,
        int DaysAdmitted,
        string Status); // Admitted, Discharged
}
