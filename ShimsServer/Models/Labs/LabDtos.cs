using System.ComponentModel.DataAnnotations;

namespace ShimsServer.Models.Labs
{
    // DTOs for Lab Requests

    /// <summary>
    /// Data transfer object for lab request information
    /// </summary>
    public record LabRequestDTO(
        Guid LabRequestsID,
        Guid PatientsAttendancesID,
        Guid SchemeLabsID,
        string PatientName,
        string LabGroupName,
        DateTime DateRequested,
        string UserName,
        bool IsPaid);

    /// <summary>
    /// Data transfer object for creating a new lab request
    /// </summary>
    public record AddLabRequestDto(
        [Required] Guid PatientsAttendancesID,
        [Required] Guid SchemeLabsID);

    /// <summary>
    /// Data transfer object for lab request summary with patient information
    /// </summary>
    public record LabRequestSummaryDto(
        Guid LabRequestsID,
        string PatientName,
        string LabGroupName,
        DateTime DateRequested,
        string UserName,
        bool IsPaid);

    // DTOs for Lab Payments

    /// <summary>
    /// Data transfer object for lab payment information
    /// </summary>
    public record LabPaymentDTO(
        Guid LabRequestsID,
        string Receipt,
        decimal Amount,
        DateTime? DatePaid,
        Guid? PaymentTypesID,
        string? PaymentReceiver,
        string UserName);

    /// <summary>
    /// Data transfer object for creating lab payment
    /// </summary>
    public record AddLabPaymentDto(
        [Required] Guid LabRequestsID,
        [Required, StringLength(20, MinimumLength = 8)] string Receipt,
        [Required, Range(0.0, double.MaxValue)] decimal Amount,
        Guid? PaymentTypesID,
        [StringLength(75, MinimumLength = 10)] string? PaymentReceiver);

    /// <summary>
    /// Data transfer object for updating lab payment
    /// </summary>
    public record UpdateLabPaymentDto(
        Guid LabRequestsID,
        [Required, StringLength(20, MinimumLength = 8)] string Receipt,
        [Required, Range(0.0, double.MaxValue)] decimal Amount,
        Guid? PaymentTypesID,
        [StringLength(75, MinimumLength = 10)] string? PaymentReceiver);

    /// <summary>
    /// Data transfer object for lab payment with request details
    /// </summary>
    public record LabPaymentDetailedDto(
        Guid LabRequestsID,
        string PatientName,
        string LabGroupName,
        string Receipt,
        decimal Amount,
        DateTime? DatePaid,
        string UserName,
        DateTime DateRequested);

    // DTOs for Lab endpoints

    /// <summary>
    /// Data transfer object for lab group information
    /// </summary>
    public record LabGroupDTO(Guid LabGroupsID, string LabGroup, string? LabDescription);

    /// <summary>
    /// Data transfer object for creating a new lab group
    /// </summary>
    public record AddLabGroupDto(
        [Required, StringLength(50, MinimumLength = 3)] string LabGroup,
        [StringLength(100, MinimumLength = 2)] string? LabDescription);

    /// <summary>
    /// Data transfer object for updating a lab group
    /// </summary>
    public record UpdateLabGroupDto(
        Guid LabGroupsID,
        [Required, StringLength(50, MinimumLength = 3)] string LabGroup,
        [StringLength(100, MinimumLength = 2)] string? LabDescription);

    /// <summary>
    /// Data transfer object for lab parameter information
    /// </summary>
    public record LabParameterDTO(Guid LabParametersID, string LabParameter, short Order, Guid LabGroupsID, string LabGroupName);

    /// <summary>
    /// Data transfer object for creating a new lab parameter
    /// </summary>
    public record AddLabParameterDto(
        [Required, StringLength(50, MinimumLength = 3)] string LabParameter,
        [Required, Range(0, 100)] short Order,
        [Required] int LabGroupsID);

    /// <summary>
    /// Data transfer object for updating a lab parameter
    /// </summary>
    public record UpdateLabParameterDto(
        Guid LabParametersID,
        [Required, StringLength(50, MinimumLength = 3)] string LabParameter,
        [Required, Range(0, 100)] short Order);

    /// <summary>
    /// Data transfer object for lab results
    /// </summary>
    public record LabResultDTO(
        Guid LabPaymentID,
        Guid LabParametersID,
        string LabParameterName,
        string Result,
        string? Notes,
        DateTime DateTested,
        string UserName);

    /// <summary>
    /// Data transfer object for adding lab results
    /// </summary>
    public record AddLabResultDto(
        [Required] Guid LabPaymentID,
        [Required] Guid LabParametersID,
        [Required, StringLength(50, MinimumLength = 1)] string Result,
        [StringLength(500, MinimumLength = 2)] string? Notes);

    /// <summary>
    /// Data transfer object for lab results with payment information
    /// </summary>
    public record LabResultWithPaymentDto(
        Guid LabPaymentID,
        string LabGroupName,
        LabResultDTO[] Results,
        decimal TotalCost,
        DateTime DateCreated);
}
