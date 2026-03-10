using System.ComponentModel.DataAnnotations;

namespace ShimsServer.Models.Services
{
    // DTOs for Services endpoints
    
    /// <summary>
    /// Data transfer object for service information
    /// </summary>
    public record ServicesDTO(int ServicesID, string Service, string ServiceGroup);

    /// <summary>
    /// Data transfer object for creating a new service
    /// </summary>
    public record AddServiceDto(
        [Required, StringLength(100)] string Service,
        [Required, StringLength(50)] string ServiceGroup);

    /// <summary>
    /// Data transfer object for updating a service
    /// </summary>
    public record UpdateServiceDto(
        int ServicesID,
        [Required, StringLength(100)] string Service,
        [Required, StringLength(50)] string ServiceGroup);

    /// <summary>
    /// Data transfer object for service with scheme coverage
    /// </summary>
    public record ServiceWithCoverageDto(
        int ServicesID,
        string Service,
        string ServiceGroup,
        decimal? Price,
        string[] SchemesCovered);
}
