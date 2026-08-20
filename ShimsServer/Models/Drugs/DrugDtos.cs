using System.ComponentModel.DataAnnotations;

namespace ShimsServer.Models.Drugs
{
    // DTOs for Drugs endpoints

    /// <summary>
    /// Data transfer object for drug information
    /// </summary>
    public record DrugsDTO(Guid DrugsID, string Drug, string? Tags, string? Description, DateTime DateAdded);

    /// <summary>
    /// Data transfer object for creating a new drug
    /// </summary>
    public record AddDrugDto(
        [Required, StringLength(150)] string Drug,
        [StringLength(100)] string? Description,
        string? Tags);

    /// <summary>
    /// Data transfer object for updating a drug
    /// </summary>
    public record UpdateDrugDto(
        Guid DrugsID,
        [Required, StringLength(150)] string Drug,
        [StringLength(100)] string? Description,
        string? Tags);

    /// <summary>
    /// Data transfer object for drug stock information
    /// </summary>
    public record DrugStockDTO(Guid DrugsStockID, Guid DrugsID, string DrugName, short Quantity, DateTime TranDate);

    /// <summary>
    /// Data transfer object for adding drug stock
    /// </summary>
    public record AddDrugStockDto(
        [Required] Guid DrugsID,
        [Required, Range(1, short.MaxValue)] short Quantity);

    /// <summary>
    /// Data transfer object for updating drug stock
    /// </summary>
    public record UpdateDrugStockDto(
        Guid DrugsStockID,
        [Required, Range(1, short.MaxValue)] short Quantity);

    /// <summary>
    /// Data transfer object for drug with current stock
    /// </summary>
    public record DrugWithStockDto(
        Guid DrugsID,
        string Drug,
        string? Description,
        short CurrentStock,
        int LowStockThreshold);

    // DTOs for Drug Requests

    /// <summary>
    /// Data transfer object for drug request information
    /// </summary>
    public record DrugRequestDTO(
        Guid DrugsRequestsID,
        Guid PatientAttendancesID,
        string Physician,
        DateTime RequestDate,
        bool IsPaid,
        bool IsDispensed);

    /// <summary>
    /// Data transfer object for a single drug detail in a prescription
    /// </summary>
    public record AddPrescriptionDetails(
        [Required] Guid SchemeDrugsID,
        [Required, Range(1, 6, ErrorMessage = "Frequency must be between 1 and 6 daily")] byte Frequency,
        [Required, Range(1, 200, ErrorMessage = "Days must be between 1 and 200")] byte Days,
        [Required, Range(0, 200)] byte QuantityRequested);

    /// <summary>
    /// Data transfer object for a single drug detail in a prescription
    /// </summary>
    public record EditPrescriptionDetails(
        [Required] Guid DrugsRequestDetailsID,
        [Required] Guid SchemeDrugsID,
        [Required, Range(1, 6, ErrorMessage = "Frequency must be between 1 and 6 daily")] byte Frequency,
        [Required, Range(1, 200, ErrorMessage = "Days must be between 1 and 200")] byte Days,
        [Required, Range(0, 200)] byte QuantityRequested);

    /// <summary>
    /// Data transfer object for creating a new drug prescription with multiple drugs
    /// </summary>
    public record AddDrugRequestDto(
        [Required] Guid PatientAttendancesID,
        [Required, MinLength(1, ErrorMessage = "At least one drug is required in a prescription")] AddPrescriptionDetails[] Drugs);

    /// <summary>
    /// Edit a drug request detail - only frequency, days and quantity can be edited
    /// </summary>
    public record EditDrugsRequestDto(
        Guid DrugsRequestDetailsID,
        [Required, Range(1, 6, ErrorMessage = "Frequency must be between {0} and {1} daily")] byte Frequency,
        [Required, Range(1, 200, ErrorMessage = "Days must be between 1 and 200")] byte Days,
        [Required, Range(0, 200)] byte QuantityRequested);

    /// <summary>
    /// Data transfer object for creating dispensing calculations
    /// </summary>
    public record AddDispensingCalculationDto(
        [Required] Guid DrugsRequestsID,
        [Required, Range(0, 100)] byte Quantity,
        [StringLength(150, MinimumLength = 10)] string? Notes);

    /// <summary>
    /// Data transfer object for updating dispensing calculations
    /// </summary>
    public record UpdateDispensingCalculationDto(
        Guid DrugsRequestsID,
        [Required, Range(0, 100)] byte Quantity,
        [StringLength(150, MinimumLength = 10)] string? Notes);

    // DTOs for Drug Payments

    /// <summary>
    /// Data transfer object for drug payment information
    /// </summary>
    public record DrugPaymentDTO(
        Guid DispensingCaculationsID,
        string Receipt,
        byte QuantityPaid,
        decimal Amount,
        DateTime? DatePaid,
        Guid? PaymentTypesID,
        string UserName);

    /// <summary>
    /// Data transfer object for creating a drug payment
    /// </summary>
    public record AddDrugPaymentDto(
        [Required] Guid DispensingCaculationsID,
        [Required, StringLength(20, MinimumLength = 8)] string Receipt,
        [Required, Range(1, 255)] byte QuantityPaid,
        [Required, Range(0, double.MaxValue)] decimal Amount,
        Guid? PaymentTypesID);

    /// <summary>
    /// Data transfer object for updating a drug payment
    /// </summary>
    public record UpdateDrugPaymentDto(
        Guid DispensingCaculationsID,
        [Required, StringLength(20, MinimumLength = 8)] string Receipt,
        [Required, Range(1, 255)] byte QuantityPaid,
        [Required, Range(0, double.MaxValue)] decimal Amount,
        Guid? PaymentTypesID);

    /// <summary>
    /// Data transfer object for drug payment with dispensing details
    /// </summary>
    public record DrugPaymentDetailedDto(
        Guid DispensingCaculationsID,
        string Receipt,
        byte QuantityPaid,
        decimal Amount,
        DateTime? DatePaid,
        byte QuantityDispensed,
        DateTime DateDispensed,
        string DispensingUserName);

    // DTOs for Dispensing

    // DTOs for Dispensing Details

    /// <summary>
    /// Data transfer object for dispensing information
    /// </summary>
    public record DispensingDTO(
        Guid DrugPaymentsID,
        DateTime DateDispensed,
        byte QuantityDispensed,
        string UserName);

    /// <summary>
    /// Data transfer object for creating dispensing record
    /// </summary>
    public record AddDispensingDto(
        [Required] Guid DrugPaymentsID,
        [Required, Range(1, 100)] byte QuantityDispensed);

    /// <summary>
    /// Data transfer object for updating dispensing record
    /// </summary>
    public record UpdateDispensingDto(
        Guid DrugPaymentsID,
        [Required, Range(1, 100)] byte QuantityDispensed);

    /// <summary>
    /// Data transfer object for dispensing detail with drug and request information
    /// </summary>
    public record DispensingDetailDto(
        Guid DispensingID,
        Guid DrugsRequestsID,
        string DrugName,
        byte QuantityDispensed,
        DateTime DateDispensed,
        string UserName);

    /// <summary>
    /// Data transfer object for dispensing calculation detail with payment information
    /// </summary>
    public record DispensingCalculationDetailDto(
        Guid DrugsRequestsID,
        byte QuantityCalculated,
        byte QuantityPaid,
        decimal PaymentAmount,
        bool IsPaid,
        DateTime? DateCalculated,
        DateTime? DatePaid);

    /// <summary>
    /// Data transfer object for complete drug workflow (request to dispensing)
    /// </summary>
    public record DrugWorkflowDTO(
        Guid DrugsRequestsID,
        string DrugName,
        byte QuantityRequested,
        byte? QuantityCalculated,
        byte? QuantityDispensed,
        decimal? PaymentAmount,
        string Status); // Requested, Calculated, Paid, Dispensed

    /// <summary>
    /// Data transfer object for a single drug in a prescription response
    /// </summary>
    public record PrescriptionDrugResponseDto(
        Guid DrugsRequestsID,
        string DrugName,
        byte Frequency,
        byte Days,
        byte QuantityRequested,
        DateTime DateRequested);

    /// <summary>
    /// Data transfer object for prescription response with all drugs
    /// </summary>
    public record PrescriptionResponseDto(
        Guid DrugsRequestsID,
        Guid PatientAttendancesID,
        string Physician,
        DateTime RequestDate,
        IEnumerable<PrescriptionDrugResponseDto> Drugs,
        bool IsPaid,
        bool IsDispensed);
}

