using Dapper;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ShimsServer.Repositories
{
    public interface IAttendanceRepository
    {
        public Task<int> AddAttendance(AddAttendanceDto dto, Guid PtID, string UserName, CancellationToken cancellationToken = default);

        public Task<PatientDetailsDto?> ActiveSessions(Guid id, CancellationToken cancellationToken = default);

        public Task<IEnumerable<PatientDetailsDto>> ActiveSessions(CancellationToken cancellationToken = default);


        public Task<int> EndSession(Guid id);

        public Task<IEnumerable<VwSessions>> GetPatientSessions(Guid id, CancellationToken cancellationToken = default);
    }

    public class AttendanceRepository(IConnection connection) : IAttendanceRepository
    {

        public async Task<int> EndSession(Guid id)
        {
            const string sql = """
                  UPDATE patientattendances 
                    SET isactive = false,
                        DateEnded = now()
                  WHERE patientattendancesid = @id;
                """;
            using var con = await connection.ConnectionAsync();
            return await con.ExecuteAsync(sql, new { id });
        }
        public async Task<IEnumerable<VwSessions>> GetPatientSessions(Guid id, CancellationToken cancellationToken = default)
        {
            const string sql =
                """
                SELECT pa.patientattendancesid, pa.visittype, pa.dateseen::date, isactive
                FROM patientattendances pa
                WHERE pa.patientsid = @id
                ORDER BY pa.dateseen DESC
                LIMIT 10
                """;

            using var con = await connection.ConnectionAsync(cancellationToken);
            return await con.QueryAsync<VwSessions>(sql, new { id });
        }

        public async Task<PatientDetailsDto?> ActiveSessions(Guid id, CancellationToken cancellationToken = default)
        {
            const string sql =
                """
                SELECT patientsid, schemesid, age, sex, fullname, schemename, hospitalid, cardid, dateofbirth, expirydate, dateseen, patientschemesid, phonenumber, visittype, ghanacard, patientattendancesid, coverage
                FROM vw_active_sessions
                WHERE patientsid = @id
                ORDER BY dateseen DESC
                LIMIT 10;
                """;
            using var con = await connection.ConnectionAsync(cancellationToken);
            return (await con.QueryAsync<PatientDetails>(sql, new { id })).ToPatientDetailsDto().FirstOrDefault();
        }

        public async Task<IEnumerable<PatientDetailsDto>> ActiveSessions(CancellationToken cancellationToken = default)
        {
            Console.WriteLine("Fetching all active sessions");
            const string sql =
                """
                SELECT patientsid, schemesid, age, sex, fullname, schemename, hospitalid, cardid, dateofbirth, expirydate, dateseen, patientschemesid, phonenumber, visittype, ghanacard, patientattendancesid, coverage
                FROM vw_active_sessions
                """;
            using var con = await connection.ConnectionAsync(cancellationToken);
            return (await con.QueryAsync<PatientDetails>(sql)).ToPatientDetailsDto();
        }

        public async Task<int> AddAttendance(AddAttendanceDto dto, Guid PtID, string user, CancellationToken cancellationToken = default)
        {
            const string sql = """
                  INSERT INTO public.patientattendances(
                    patientattendancesid, patientsid, visittype, dateseen, username, isactive)
                  VALUES (@ptid, @ptsid, @vtype, now(), @user, true);
                """;
            using var con = await connection.ConnectionAsync(cancellationToken);
            using var tran = await con.BeginTransactionAsync(cancellationToken);
            await con.ExecuteAsync("UPDATE patientattendances SET isactive = false WHERE patientsid = @ptsid AND isactive = true;", new { ptsid = dto.PatientsID }, transaction: tran);
            var result = await con.ExecuteAsync(sql, new
            {
                PtID,
                ptsid = dto.PatientsID,
                vtype = dto.VisitType,
                user
            }, transaction: tran);
            Console.WriteLine($"Added new attendance record for patient {dto.PatientsID} with visit type {dto.VisitType}: {result} rows affected");
            await tran.CommitAsync(cancellationToken);
            return result;
        }
    }


    public record AddAttendanceDto(
     Guid PatientsID,

    [StringLength(15)]
    [DefaultValue("Acute")]
    [AllowedValues(["Acute", "Review", "Chronic"])]
    string VisitType
    );
}
