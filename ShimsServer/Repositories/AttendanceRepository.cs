using Dapper;
using Npgsql;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ShimsServer.Repositories
{
    public interface IAttendanceRepository
    {
        public Task<int> AddAttendance(AddAttendanceDto dto, Guid PtID, string UserName, CancellationToken cancellationToken = default);

        public Task<IEnumerable<ListPatientsDto>> ActiveSessions(Guid id, CancellationToken cancellationToken = default);
    }

    public class AttendanceRepository(IConnection connection) : IAttendanceRepository
    {
        public async Task<IEnumerable<ListPatientsDto>> ActiveSessions(Guid id, CancellationToken cancellationToken = default)
        {
            const string sql =
                """
                    SELECT patientid, schemesid, age, gender, fullname, scheme, hospitalid, cardid, expirydate, attendancedate
                    FROM vw_patients
                    WHERE patientid = @id;
                """;
            using var con = await connection.ConnectionAsync(cancellationToken);
            return await con.QueryAsync<ListPatientsDto>(sql, new { id });
        }

        public async Task<int> AddAttendance(AddAttendanceDto dto, Guid PtID, string user, CancellationToken cancellationToken = default)
        {
            const string sql = """
                  UPDATE patientattendances SET isactive = false
                  WHERE patientschemesid = @ptsid;
                  INSERT INTO public.patientattendances(
                    patientattendancesid, patientschemesid, visittype, dateseen, username, isactive)
                  VALUES (@ptid, @ptsid, @vtype, now(), @user, true);
                """;
            using var con = await connection.ConnectionAsync(cancellationToken);
            using var tran = await con.BeginTransactionAsync(cancellationToken);
            var result = await con.ExecuteAsync(sql, new
            {
                PtID,
                ptsid = dto.PatientSchemesID,
                vtype = dto.VisitType,
                user
            }, transaction: tran);
            await tran.CommitAsync(cancellationToken);
            return result;
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
