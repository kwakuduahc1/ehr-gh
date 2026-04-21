using Microsoft.AspNetCore.Mvc;
using Npgsql;
using ShimsServer.Repositories;

namespace ShimsServer.Controllers.Records
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]

    public class AttendancesController(IAttendanceRepository dataSource, ILogger<AttendancesController> logger) : ControllerBase
    {
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<ListPatientsDto>>> GetActiveSessions(Guid id)
        {
            var sessions = await dataSource.ActiveSessions(id, HttpContext.RequestAborted);
            return Ok(sessions);
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<int>> AddAttendance([FromBody] AddAttendanceDto dto)
        {
            var userName = User.Identity?.Name ?? "system";
            var ptId = Guid.CreateVersion7();
            try
            {
                var attendanceId = await dataSource.AddAttendance(dto, ptId, userName, HttpContext.RequestAborted);
                return Ok(attendanceId);
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
    }
}
