using System.ComponentModel.DataAnnotations;

namespace ShimsServer.Models.Investigations
{
    // DTOs for Investigation Requests

    /// <summary>
    /// Data transfer object for investigation request information
    /// SELECT ir.investigationsrequestsid, ir.investigationsid, ir.daterequested, i.investigation
    /// FROM investigationsrequests ir
    /// INNER JOIN investigations i ON i.investigationsid = ir.investigationsid
    /// </summary>
    public record PatientAttendanceInvestigations(
        Guid InvestigationSRequestsID,
        Guid InvestigationsID,
        DateOnly DateRequested,
        string Investigation);

    /// <summary>
    /// Data transfer object for creating a new investigation request
    /// </summary>
    public record AddInvestigationRequestDto(
        [Required] Guid PatientsAttendancesID,
        [Required] Guid SchemeInvestigationsID);

    /// <summary>
    /// Data transfer object for investigation request summary with patient information
    /// </summary>
    public record InvestigationRequestSummaryDto(
        Guid InvestigationRequestsID,
        string PatientName,
        string InvestigationGroupName,
        DateTime DateRequested,
        string UserName,
        bool IsPaid);

    // DTOs for Investigation Payments

    /// <summary>
    /// Data transfer object for investigation payment information
    /// </summary>
    public record InvestigationPaymentDTO(
        Guid InvestigationRequestsID,
        string Receipt,
        decimal Amount,
        DateTime? DatePaid,
        Guid? PaymentTypesID,
        string? PaymentReceiver,
        string UserName);

    /// <summary>
    /// Data transfer object for creating investigation payment
    /// </summary>
    public record AddInvestigationPaymentDto(
        [Required] Guid InvestigationRequestsID,
        [Required, StringLength(20, MinimumLength = 8)] string Receipt,
        [Required, Range(0.0, double.MaxValue)] decimal Amount,
        Guid? PaymentTypesID,
        [StringLength(75, MinimumLength = 10)] string? PaymentReceiver);

    /// <summary>
    /// Data transfer object for updating investigation payment
    /// </summary>
    public record UpdateInvestigationPaymentDto(
        Guid InvestigationRequestsID,
        [Required, StringLength(20, MinimumLength = 8)] string Receipt,
        [Required, Range(0.0, double.MaxValue)] decimal Amount,
        Guid? PaymentTypesID,
        [StringLength(75, MinimumLength = 10)] string? PaymentReceiver);

    /// <summary>
    /// Data transfer object for investigation payment with request details
    /// </summary>
    public record InvestigationPaymentDetailedDto(
        Guid InvestigationRequestsID,
        string PatientName,
        string InvestigationGroupName,
        string Receipt,
        decimal Amount,
        DateTime? DatePaid,
        string UserName,
        DateTime DateRequested);

    // DTOs for Investigation endpoints

    /// <summary>
    /// Data transfer object for investigation group information
    /// </summary>
    public record InvestigationGroupDTO(Guid InvestigationGroupsID, string InvestigationGroup, string? InvestigationDescription);

    /// <summary>
    /// Data transfer object for creating a new investigation group
    /// </summary>
    public record AddInvestigationGroupDto(
        [Required, StringLength(50, MinimumLength = 3)] string InvestigationGroup,
        [StringLength(100, MinimumLength = 2)] string? InvestigationDescription);

    /// <summary>
    /// Data transfer object for updating an investigation group
    /// </summary>
    public record UpdateInvestigationGroupDto(
        Guid InvestigationGroupsID,
        [Required, StringLength(50, MinimumLength = 3)] string InvestigationGroup,
        [StringLength(100, MinimumLength = 2)] string? InvestigationDescription);

    /// <summary>
    /// Data transfer object for investigation parameter information
    /// </summary>
    public record InvestigationParameterDTO(Guid InvestigationParametersID, string InvestigationParameter, short Order, Guid InvestigationGroupsID, string InvestigationGroupName);

    /// <summary>
    /// Data transfer object for creating a new investigation parameter
    /// </summary>
    public record AddInvestigationParameterDto(
        [Required, StringLength(50, MinimumLength = 3)] string InvestigationParameter,
        [Required, Range(0, 100)] short Order,
        [Required] int InvestigationGroupsID);

    /// <summary>
    /// Data transfer object for updating an investigation parameter
    /// </summary>
    public record UpdateInvestigationParameterDto(
        Guid InvestigationParametersID,
        [Required, StringLength(50, MinimumLength = 3)] string InvestigationParameter,
        [Required, Range(0, 100)] short Order);

    /// <summary>
    /// Data transfer object for investigation results
    /// </summary>
    public record InvestigationResultDTO(
        Guid InvestigationPaymentID,
        Guid InvestigationParametersID,
        string InvestigationParameterName,
        string Result,
        string? Notes,
        DateTime DateTested,
        string UserName);

    /// <summary>
    /// Data transfer object for adding investigation results
    /// </summary>
    public record AddInvestigationResultDto(
        [Required] Guid InvestigationPaymentID,
        [Required] Guid InvestigationParametersID,
        [Required, StringLength(50, MinimumLength = 1)] string Result,
        [StringLength(500, MinimumLength = 2)] string? Notes);

    /// <summary>
    /// Data transfer object for investigation results with payment information
    /// </summary>
    public record InvestigationResultWithPaymentDto(
        Guid InvestigationPaymentID,
        string InvestigationGroupName,
        InvestigationResultDTO[] Results,
        decimal TotalCost,
        DateTime DateCreated);
}
