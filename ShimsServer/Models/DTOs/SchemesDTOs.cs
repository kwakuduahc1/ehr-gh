using System.ComponentModel.DataAnnotations;
namespace ShimsServer.Models.DTOs;

public record SchemesDTO(Guid SchemesID, string SchemeName, string Coverage, decimal MaxPayable, decimal Recovery);

public record AddSchemeDto(
    string SchemeName,
    [StringLength(30, MinimumLength = 3), AllowedValues(["Full", "Relative", "Fixed"])] string Coverage,
    [Range(0.01, double.MaxValue, ErrorMessage = "MaxPayable must be greater than 0")] decimal MaxPayable,
    [Range(0, double.MaxValue)] decimal Recovery);

public record UpdateSchemeDto(
    Guid SchemesID,
    string SchemeName,
    [StringLength(30, MinimumLength = 3), AllowedValues(["Full", "Relative", "Fixed"])] string Coverage,
    [Range(0.01, double.MaxValue, ErrorMessage = "MaxPayable must be greater than 0")] decimal MaxPayable,
    [Range(0, double.MaxValue)] decimal Recovery);