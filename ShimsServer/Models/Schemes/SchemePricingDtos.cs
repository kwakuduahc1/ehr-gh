using System.ComponentModel.DataAnnotations;

namespace ShimsServer.Models.Schemes
{
    // DTOs for Scheme Drugs (Scheme - Drug pricing mapping)

    /// <summary>
    /// Data transfer object for scheme drug pricing information
    /// </summary>
    public record SchemeDrugDTO(
        Guid SchemeDrugsID,
        string Drug,
        decimal Price,
        string DrugCode,
        string Tags,
        string? Description);

    /// <summary>
    /// Data transfer object for creating scheme drug pricing
    /// </summary>
    public record AddSchemeDrugDto(
        Guid SchemesID,
        Guid DrugsID,
        [Range(0.5D, double.MaxValue, ErrorMessage = "Price must be greater than 0.5")] decimal Price);

    /// <summary>
    /// Data transfer object for updating scheme drug pricing
    /// </summary>
    public record UpdateSchemeDrugDto(
        Guid SchemeDrugsID,
        Guid SchemesID,
        Guid DrugsID,
        [Range(0.5D, double.MaxValue, ErrorMessage = "Price must be greater than 0.5")] decimal Price);

    /// <summary>
    /// Data transfer object for scheme drugs with availability status
    /// </summary>
    public record SchemeDrugAvailabilityDto(
        Guid SchemeDrugsID,
        string SchemeName,
        string DrugName,
        decimal Price,
        bool IsAvailable);

    // DTOs for Scheme Labs (Scheme - Lab pricing mapping)

    /// <summary>
    /// Data transfer object for scheme lab pricing information
    /// </summary>
    public record SchemeLabDTO(
        Guid SchemeLabsID,
        Guid SchemesID,
        Guid LabsGroupID,
        string SchemeName,
        string LabGroupName,
        decimal Price,
        DateTime DateSet);

    /// <summary>
    /// Data transfer object for creating scheme lab pricing
    /// </summary>
    public record AddSchemeLabDto(
        Guid SchemesID,
        Guid LabsGroupID,
        [Range(0.5D, double.MaxValue, ErrorMessage = "Price must be greater than 0.5")] decimal Price);

    /// <summary>
    /// Data transfer object for updating scheme lab pricing
    /// </summary>
    public record UpdateSchemeLabDto(
        Guid SchemeLabsID,
        Guid SchemesID,
        Guid LabsGroupID,
        [Range(0.5D, double.MaxValue, ErrorMessage = "Price must be greater than 0.5")] decimal Price);

    /// <summary>
    /// Data transfer object for scheme labs with available tests
    /// </summary>
    public record SchemeLabAvailabilityDto(
        Guid SchemeLabsID,
        string SchemeName,
        string LabGroupName,
        decimal Price,
        int AvailableTests);

    // DTOs for Scheme Services (Scheme - Service pricing mapping)

    /// <summary>
    /// Data transfer object for scheme service pricing information
    /// </summary>
    public record SchemeServiceDTO(
        Guid SchemeServicesID,
        Guid ServicesID,
        decimal Price,
        Array Tiers,
        string? GDRG,
        string? Narration,
        string Service,
        string ServiceGroup);

    /// <summary>
    /// Data transfer object for creating scheme service pricing
    /// </summary>
    public record AddSchemeServiceDto(
        Guid SchemesID,
        Guid ServicesID,
       [StringLength(20)] string GDRG,
       [StringLength(150)] string Narration,
       string[] AllowedTiers,
        [Range(0.5D, double.MaxValue, ErrorMessage = "Price must be greater than 0.5")] decimal Price);

    /// <summary>
    /// Data transfer object for updating scheme service pricing
    /// </summary>
    public record UpdateSchemeServiceDto(
        Guid SchemeServicesID,
        Guid SchemesID,
        Guid ServicesID,
        [Range(0.5D, double.MaxValue, ErrorMessage = "Price must be greater than 0.5")] decimal Price);

    /// <summary>
    /// Data transfer object for scheme services with coverage status
    /// </summary>
    public record SchemeServiceAvailabilityDto(
        Guid SchemeServicesID,
        string SchemeName,
        string ServiceName,
        string ServiceGroup,
        decimal Price,
        bool IsCovered);
}
