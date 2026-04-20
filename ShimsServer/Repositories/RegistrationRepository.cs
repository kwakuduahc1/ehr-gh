using Dapper;
using Npgsql;
using System.ComponentModel.DataAnnotations;

namespace ShimsServer.Repositories
{
    public interface IRegistrationRepository
    {
        Task<NpgsqlConnection> ConnectionAsync(CancellationToken cancellationToken = default);

        Task AddPatientAsync(AddPatientDto dto, (Guid PatientsID, Guid PatientAttendancesID, string UserName) ids, CancellationToken cancellationToken = default);

        Task<int> EditPatientAsync(EditPatientDto dto, string UserName, CancellationToken cancellationToken = default);

        Task DeletePatientAsync(Guid id, CancellationToken cancellationToken = default);

        Task<IEnumerable<ListPatientsDto>> GetPatientsAsync(CancellationToken cancellationToken = default);

        Task<bool> PatientExists(Guid id, CancellationToken cancellationToken = default);

        Task<ListPatientsDto?> GetPatientByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<IEnumerable<ListPatientsDto>> SearchPatientsAsync(string search, CancellationToken cancellationToken = default);
    }

    public class RegistrationRepository(NpgsqlDataSource dsource) : IRegistrationRepository
    {
        public async Task AddPatientAsync(AddPatientDto dto, (Guid PatientsID, Guid PatientAttendancesID, string UserName) ids, CancellationToken cancellationToken = default)
        {
            const string sql =
                """
                    INSERT INTO Patients (PatientID, Surname, OtherNames, DateOfBirth, GhanaCard, Sex, PhoneNumber, UserName)
                    VALUES (@PatientsID, @Surname, @OtherNames, @DateOfBirth, @GhanaCard, @Sex, @PhoneNumber, @UserName);

                    INSERT INTO PatientSchemes (PatientSchemesID, PatientsID, HospitalID, SchemesID, CardID, ExpiryDate, LastUpdateDate, UserName, IsActive)
                    VALUES (uuidv7(), @PatientsID, generate_hospital_id(), @SchemesID, @CardID, @ExpiryDate, now(), @UserName, true);

                    INSERT INTO PatientAttendances(PatientAttendancesID, PatientsID, VisitType, UserName, DateSeen)
                    VALUES (@PatientAttendancesID, @PatientsID, 'Acute', @UserName, now());
                """;

            using var con = await ConnectionAsync(cancellationToken);
            using var transaction = await con.BeginTransactionAsync(cancellationToken);
            using var res = await con.QueryMultipleAsync(sql, new
            {
                ids.PatientsID,
                dto.Surname,
                dto.OtherNames,
                dto.DateOfBirth,
                dto.GhanaCard,
                dto.Sex,
                dto.SchemesID,
                dto.CardID,
                dto.ExpiryDate,
                ids.PatientAttendancesID,
                dto.PhoneNumber,
                ids.UserName
            });
            await transaction.CommitAsync(cancellationToken);
        }

        public async Task DeletePatientAsync(Guid id, CancellationToken cancellationToken = default)
        {
            const string sql =
                """
                UPDATE Patients SET IsActive = false
                WHERE PatientsID = @id;

                UPDATE PatientSchemes SET IsActive = false
                WHERE PatientsID = @id;

                UPDATE PatientAttendances SET IsActive = false
                WHERE PatientsID = @id;
                """;
            using var con = await ConnectionAsync(cancellationToken);
            using var transaction = await con.BeginTransactionAsync(cancellationToken);
            using var res = await con.QueryMultipleAsync(sql, new { id });
            await transaction.CommitAsync(cancellationToken);
        }

        public async Task<int> EditPatientAsync(EditPatientDto dto, string UserName, CancellationToken cancellationToken = default)
        {
            const string sql =
                 """
                    UPDATE Patients
                    SET Surname = @Surname,
                        OtherNames = @OtherNames,
                        DateOfBirth = @DateOfBirth,
                        GhanaCard = @GhanaCard,
                        PhoneNumber = @PhoneNumber,
                        Sex = @Sex,
                        UserName = @UserName
                    WHERE PatientID = @PatientID;
                """;
            using var con = await ConnectionAsync(cancellationToken);
            using var transaction = await con.BeginTransactionAsync(cancellationToken);
            var n = await con.ExecuteAsync(sql, new
            {
                dto.PatientID,
                dto.Surname,
                dto.OtherNames,
                dto.DateOfBirth,
                dto.GhanaCard,
                dto.PhoneNumber,
                dto.Sex,
                UserName
            });
            await transaction.CommitAsync(cancellationToken);
            return n;
        }

        public async Task<NpgsqlConnection> ConnectionAsync(CancellationToken cancellationToken = default)
        {
            var connection = dsource.CreateConnection();
            await connection.OpenAsync(cancellationToken);
            return connection;
        }

        public async Task<ListPatientsDto?> GetPatientByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            const string sql =
                """
                    SELECT patientid, schemesid, age, gender, fullname, scheme, hospitalid, cardid, expirydate, attendancedate
                    FROM vw_patients
                    WHERE patientid = @id;
                """;
            using var con = await ConnectionAsync(cancellationToken);
            return await con.QueryFirstOrDefaultAsync<ListPatientsDto>(sql, new { id });
        }

        public async Task<IEnumerable<ListPatientsDto>> GetPatientsAsync(CancellationToken cancellationToken = default)
        {
            const string sql =
                """
                    SELECT patientid, schemesid, age, gender, fullname, scheme, hospitalid, cardid, expirydate, attendancedate
                    FROM vw_patients;
                """;
            using var con = await ConnectionAsync(cancellationToken);
            return await con.QueryAsync<ListPatientsDto>(sql);
        }

        public async Task<bool> PatientExists(Guid id, CancellationToken cancellationToken = default)
        {
            const string sql =
                """
                    SELECT EXISTS(SELECT 1 FROM Patients WHERE PatientsID = @id AND IsActive = true);
                """;
            using var con = await ConnectionAsync(cancellationToken);
            return await con.ExecuteScalarAsync<bool>(sql, new { id });
        }

        public async Task<IEnumerable<ListPatientsDto>> SearchPatientsAsync(string search, CancellationToken cancellationToken = default)
        {
            const string sql =
                """
                    SELECT patientid, schemesid, age, gender, fullname, scheme, hospitalid, cardid, expirydate, attendancedate
                    FROM vw_patients
                    WHERE fullname ILIKE @search
                        OR cardid ILIKE @search
                        OR hospitalid ILIKE @search
                    ORDER BY attendancedate DESC;
                """;
            using var con = await ConnectionAsync(cancellationToken);
            return await con.QueryAsync<ListPatientsDto>(sql, new { search = $"%{search}%" });
        }
    }

    public record AddPatientDto(
        [StringLength(30, MinimumLength = 3)] string Surname,

        [StringLength(50, MinimumLength = 3)] string OtherNames,

        DateTime DateOfBirth,
        [StringLength(17), RegularExpression(@"^GHA-\d{10}-(?:0[1-9]|1[0-6])$", ErrorMessage = "GhanaCard must follow the format: GHA-xxxxxxxxxx-01 to 16")] string? GhanaCard,

        [StringLength(6, MinimumLength = 4), AllowedValues("Male", "Female")] string Sex,

        Guid SchemesID,
        [StringLength(30, MinimumLength = 10)] string CardID,

        DateTime ExpiryDate,
        [DataType(DataType.PhoneNumber)] string? PhoneNumber
);

    public record EditPatientDto(
    [Required] Guid PatientID,

    [Required] string HospitalID,

    [StringLength(17), RegularExpression(@"^GHA-\d{10}-(?:0[1-9]|1[0-6])$", ErrorMessage = "GhanaCard must follow the format: GHA-xxxxxxxxxx-01 to 16")] string? GhanaCard,

    [Required, StringLength(30, MinimumLength = 3)] string Surname,

    [Required, StringLength(50, MinimumLength = 3)] string OtherNames,

    [Required] DateTime DateOfBirth,

    [Required, StringLength(6, MinimumLength = 4), AllowedValues("Male", "Female")] string Sex,

    [Required, DataType(DataType.PhoneNumber)] string PhoneNumber);

    public record ListPatientsDto(
        Guid PatientID,
        Guid SchemesID,
        short Age,
        string Gender,
        string FullName,
        string Scheme,
        string HospitalID,
        string CardID,
        DateTime ExpiryDate,
        string VisitType,
        DateTime AttendanceDate
        );
}
