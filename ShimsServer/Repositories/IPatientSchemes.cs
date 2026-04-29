using Dapper;
using ShimsServer.Models.Records;

namespace ShimsServer.Repositories
{
    public interface IPatientSchemesRepository
    {
        Task<int> AddPatientScheme(AddPatientSchemeDto scheme, (Guid id, string username) user, CancellationToken cancellationToken = default);

        Task<bool> PatientHasScheme(Guid id, Guid psid, CancellationToken token);

        Task<int> EditPatientScheme(EditPatientSchemeDto scheme, string username, CancellationToken cancellationToken = default);

        Task<int> DeletePatientScheme(Guid id, CancellationToken cancellationToken = default);

        Task<bool> PatientSchemeExists(Guid id, CancellationToken cancellationToken = default);
    }

    public class PatientSchemesRepository(IConnection connection) : IPatientSchemesRepository
    {

        public async Task<bool> PatientHasScheme(Guid id, Guid psid, CancellationToken token)
        {
            const string sql = 
                """
                    SELECT EXISTS(
                    SELECT true
                    FROM patientschemes
                    WHERE patientsid = @id AND patientschemesid = @psid AND isactive = true
                    );
                 """;
            using var con = await connection.ConnectionAsync(token);
            return await con.ExecuteScalarAsync<bool>(sql, new { id, psid });
        }

        public async Task<bool> PatientSchemeExists(Guid id, CancellationToken cancellationToken = default)
        {
            const string sql = 
                """
                   SELECT EXISTS(
                    SELECT true
                    FROM patientschemes
                    WHERE patientschemesid = @id
                   );
                 """;
            using var con = await connection.ConnectionAsync(cancellationToken);
            return await con.ExecuteScalarAsync<bool>(sql, new { id });
        }

        public async Task<int> AddPatientScheme(AddPatientSchemeDto scheme, (Guid id, string username) user, CancellationToken cancellationToken = default)
        {
            const string sql = """
                    INSERT INTO patientschemes (patientschemesid, patientsid, schemesid, cardid, isactive, expirydate, username, lastupdatedate)
                    VALUES (@PatientSchemesID, @PatientsID, @SchemesID, @CardID, @IsActive, @ExpiryDate, @UserName, now());
                    """;
            using var con = await connection.ConnectionAsync(cancellationToken);
            return await con.ExecuteAsync(sql, new
            {
                PatientSchemesID = user.id,
                scheme.PatientsID,
                scheme.SchemesID,
                scheme.CardID,
                scheme.IsActive,
                scheme.ExpiryDate,
                UserName = user.username
            });
        }

        public async Task<int> EditPatientScheme(EditPatientSchemeDto scheme, string username, CancellationToken cancellationToken = default)
        {
            const string sql = """
                    UPDATE patientschemes
                    SET patientsid = @pid,
                        schemesid = @sid,
                        cardid = @cid,
                        isactive = true,
                        expirydate = @exdate,
                        username = @user,
                        lastupdatedate = now()
                    WHERE patientschemesid = @psid;
                    """;
            using var con = await connection.ConnectionAsync(cancellationToken);
          return  await con.ExecuteAsync(sql, new
            {
                psid = scheme.PatientSchemesID,
                pid = scheme.PatientsID,
                sid = scheme.SchemesID,
                cid = scheme.CardID,
                exdate = scheme.ExpiryDate,
                user = username
            });
        }

        public async Task<int> DeletePatientScheme(Guid id, CancellationToken cancellationToken = default)
        {
            const string sql = """
                    UPDATE patientschemes
                    SET isactive = false,
                        lastupdatedate = now()
                    WHERE patientschemesid = @id;
                    """;
            using var con = await connection.ConnectionAsync(cancellationToken);
          return  await con.ExecuteAsync(sql, new { id });
        }
    }
}
