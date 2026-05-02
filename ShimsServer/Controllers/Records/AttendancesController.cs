using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using Npgsql;
using ShimsServer.Repositories;

namespace ShimsServer.Controllers.Records
{
    [ApiController]
    [Produces("application/json")]
    [ApiVersion("1.0")]
    [Route("api/[controller]")]
    [Route("api/v{version:apiVersion}/[controller]")]

    public class AttendancesController(IAttendanceRepository dataSource, ILogger<AttendancesController> logger) : ControllerBase
    {
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(IEnumerable<VwSessions>), StatusCodes.Status200OK)]
        public async Task<IEnumerable<VwSessions>> GetPatientSessions(Guid id) => await dataSource.GetPatientSessions(id, HttpContext.RequestAborted);

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ListPatientsDto>), StatusCodes.Status200OK)]
        public async Task<IEnumerable<ListPatientsDto>> GetActiveSessions(Guid id) => await dataSource.ActiveSessions(id, HttpContext.RequestAborted);

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<Guid>> AddAttendance([FromBody] AddAttendanceDto dto)
        {
            var userName = User.Identity?.Name ?? "system";
            var ptId = Guid.CreateVersion7();
            try
            {
                var attendanceId = await dataSource.AddAttendance(dto, ptId, userName, HttpContext.RequestAborted);
                if (attendanceId != 1)
                    return BadRequest(new { message = "No attendance was added. Please check the input and try again." });
                return Ok(ptId);
            }
            catch (PostgresException ex)
            {
                logger.LogError(ex, "Database error adding attendance");
                return BadRequest(new { message = "There was a database level error" });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error adding attendance");
                return BadRequest(new { message = "An error occurred during attendance addition" });
            }
        }

        [HttpPut("{id:guid:required}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<int>> EndSession(Guid id)
        {
            try
            {
                if(await dataSource.EndSession(id) != 1)
                    return BadRequest(new { message = "No session was ended. Please check the session ID and try again." });
                return Ok();
            }
            catch (PostgresException ex)
            {
                logger.LogError(ex, "Database error ending session");
                return BadRequest(new { message = "There was a database level error" });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error ending session");
                return BadRequest(new { message = "An error occurred during session ending" });
            }
        }
    }
}