using Dapper;
using Npgsql;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ShimsServer.Repositories
{
    public interface IAttendanceRepository
    {
        public Task<int> AddAttendace(AddAttendanceDto dto, Guid PtID, string UserName);

        public Task<ListPatientsDto> ActiveSessions(Guid id);

        Task<NpgsqlConnection> ConnectionAsync();
    }

    public class AttendanceRepository(NpgsqlDataSource dsource, CancellationToken token=default): IAttendanceRepository
    {
        /// <summary>
        ///  Use GetPatientByIdAsync method on the registration repository
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<ListPatientsDto> ActiveSessions(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<int> AddAttendace(AddAttendanceDto dto, Guid PtID, string user)
        {
            const string sql = """
                  INSERT INTO public.patientattendances(
                	patientattendancesid, patientschemesid, visittype, dateseen, username, isactive)
                  VALUES (@ptid, @ptsid, @vtype, now(), @user, now());
                """;
            using var con = await ConnectionAsync();
            using var tran = await con.BeginTransactionAsync(token);
            return await con.ExecuteScalarAsync<int>(sql, new
            {
                PtID,
                ptsid = dto.PatientSchemesID,
                vtype = dto.VisitType,
                user
            });
        }

        public async Task<NpgsqlConnection> ConnectionAsync()
        {
            var connection = dsource.CreateConnection();
            await connection.OpenAsync(token);
            return connection;
        }
    }


    public record AddAttendanceDto(
     Guid PatientSchemesID,

    [StringLength(15)]
    [DefaultValue("Acute")]
    [AllowedValues(["Acute", "Review", "Follow-up"])]
    string VisitType
    );
}
