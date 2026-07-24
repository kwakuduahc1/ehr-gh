using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using ShimsServer.Repositories;

namespace ShimsServer.Controllers.Dispensary
{
    [ApiController]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class DispensaryController(IConsultationsRepository dataSource, ILogger<ConsultationsController> logger) : ControllerBase
    {
    }
}
