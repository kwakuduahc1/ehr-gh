using Dapper;
using System.ComponentModel.DataAnnotations;

namespace ShimsServer.Repositories
{
    public interface IRegistrationRepository
    {
        Task<string> AddPatientAsync(AddPatientDto dto, (Guid PatientsID, Guid PatientAttendancesID, Guid[] PatientSchemesID, string UserName) ids, CancellationToken cancellationToken = default);

        Task<int> EditPatientAsync(EditPatientDto dto, string UserName, CancellationToken cancellationToken = default);

        Task DeletePatientAsync(Guid id, CancellationToken cancellationToken = default);

        Task<IEnumerable<ListPatientsDto>> GetPatientsAsync(CancellationToken cancellationToken = default);

        Task<bool> PatientExists(Guid id, CancellationToken cancellationToken = default);

        Task<ListPatientsDto?> GetPatientByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<IEnumerable<ListPatientsDto>> SearchPatientsAsync(string search, CancellationToken cancellationToken = default);
    }

    public class RegistrationRepository(IConnection connection) : IRegistrationRepository
    {
        public async Task<string> AddPatientAsync(AddPatientDto dto, (Guid PatientsID, Guid PatientAttendancesID, Guid[] PatientSchemesID, string UserName) ids, CancellationToken cancellationToken = default)
        {
            var sqlBuilder = new System.Text.StringBuilder(
                """
                    INSERT INTO Patients (PatientsID, Surname, OtherNames, DateOfBirth, GhanaCard, Sex, PhoneNumber, UserName, HospitalID, IsActive)
                    VALUES (@PatientsID, @Surname, @OtherNames, @DateOfBirth, @GhanaCard, @Sex, @PhoneNumber, @UserName, generate_hospital_id(), true)
                    RETURNING HospitalID;
                """);

            for (int i = 0; i < dto.Schemes?.Length; i++)
            {
                sqlBuilder.Append($"""
                    INSERT INTO PatientSchemes (PatientSchemesID, PatientsID, IsActive, SchemesID, CardID, ExpiryDate, LastUpdateDate, UserName)
                    VALUES (@PatientSchemesID{i}, @PatientsID, true, @SchemesID{i}, @CardID{i}, @ExpiryDate{i}, now(), @UserName);
                """);
            }

            sqlBuilder.Append("""
                    INSERT INTO PatientAttendances(PatientAttendancesID, PatientsID, VisitType, UserName, DateSeen, IsActive)
                    VALUES (@PatientAttendancesID, @PatientsID, 'Acute', @UserName, now(), true);
                """);

            string sql = sqlBuilder.ToString();
            var parameters = new DynamicParameters();
            parameters.Add("@PatientsID", ids.PatientsID);
            parameters.Add("@Surname", dto.Surname);
            parameters.Add("@OtherNames", dto.OtherNames);
            parameters.Add("@DateOfBirth", dto.DateOfBirth);
            parameters.Add("@GhanaCard", dto.GhanaCard);
            parameters.Add("@Sex", dto.Sex);
            parameters.Add("@PatientAttendancesID", ids.PatientAttendancesID);
            parameters.Add("@PhoneNumber", dto.PhoneNumber);
            parameters.Add("@UserName", ids.UserName);

            for (int i = 0; i < ids.PatientSchemesID.Length; i++)
            {
                parameters.Add($"@PatientSchemesID{i}", ids.PatientSchemesID[i]);
                parameters.Add($"@SchemesID{i}", dto.Schemes[i].SchemesID);
                parameters.Add($"@CardID{i}", dto.Schemes[i].CardID);
                parameters.Add($"@ExpiryDate{i}", dto.Schemes[i].ExpiryDate);
            }

            using var con = await connection.ConnectionAsync(cancellationToken);
            using var transaction = await con.BeginTransactionAsync(cancellationToken);
            using var res = await con.QueryMultipleAsync(sql, parameters, transaction);
            var hid = await res.ReadFirstOrDefaultAsync<string>();
            await transaction.CommitAsync(cancellationToken);
            return hid ?? string.Empty;
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
            using var con = await connection.ConnectionAsync(cancellationToken);
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
                    WHERE PatientsID = @PatientsID;
                """;
            using var con = await connection.ConnectionAsync(cancellationToken);
            using var transaction = await con.BeginTransactionAsync(cancellationToken);
            var n = await con.ExecuteAsync(sql, new
            {
                dto.PatientsID,
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

        public async Task<ListPatientsDto?> GetPatientByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            const string sql =
                """
                    SELECT PatientsID, schemesid, age, gender, fullname, scheme, hospitalid, cardid, expirydate, attendancedate
                    FROM vw_patients
                    WHERE PatientsID = @id;
                """;
            using var con = await connection.ConnectionAsync(cancellationToken);
            return await con.QueryFirstOrDefaultAsync<ListPatientsDto>(sql, new { id });
        }

        public async Task<IEnumerable<ListPatientsDto>> GetPatientsAsync(CancellationToken cancellationToken = default)
        {
            const string sql =
                """
                    SELECT PatientsID, schemesid, age, gender, fullname, scheme, hospitalid, cardid, expirydate, attendancedate, patientschemesid
                    FROM vw_patients;
                """;
            using var con = await connection.ConnectionAsync(cancellationToken);
            return await con.QueryAsync<ListPatientsDto>(sql);
        }

        public async Task<bool> PatientExists(Guid id, CancellationToken cancellationToken = default)
        {
            const string sql =
                """
                    SELECT EXISTS(SELECT 1 FROM Patients WHERE PatientsID = @id AND IsActive = true);
                """;
            using var con = await connection.ConnectionAsync(cancellationToken);
            return await con.ExecuteScalarAsync<bool>(sql, new { id });
        }

        public async Task<IEnumerable<ListPatientsDto>> SearchPatientsAsync(string search, CancellationToken cancellationToken = default)
        {
            const string sql =
                """
                    SELECT PatientsID, schemesid, age, gender, fullname, scheme, hospitalid, cardid, expirydate, attendancedate, patientschemesid
                    FROM vw_patients
                    WHERE fullname ILIKE @search
                        OR cardid ILIKE @search
                        OR hospitalid ILIKE @search
                    ORDER BY attendancedate DESC;
                """;
            using var con = await connection.ConnectionAsync(cancellationToken);
            return await con.QueryAsync<ListPatientsDto>(sql, new { search = $"%{search}%" });
        }
    }

    public record InsuranceInformation(
        Guid SchemesID,
        [StringLength(30, MinimumLength = 10)] string? CardID,
        DateTime? ExpiryDate
        );

    public record AddPatientDto(
        [StringLength(30, MinimumLength = 3)] string Surname,

        [StringLength(50, MinimumLength = 3)] string OtherNames,

        DateTime DateOfBirth,
        [StringLength(17), RegularExpression(@"^GHA-\d{10}-(?:0[1-9]|1[0-6])$", ErrorMessage = "GhanaCard must follow the format: GHA-xxxxxxxxxx-01 to 16")] string? GhanaCard,

        [StringLength(6, MinimumLength = 4), AllowedValues("Male", "Female")] string Sex,

        [DataType(DataType.PhoneNumber)] string? PhoneNumber,

        InsuranceInformation[] Schemes
);

    public record EditPatientDto(
    [Required] Guid PatientsID,

    [Required] string HospitalID,

    [StringLength(17), RegularExpression(@"^GHA-\d{10}-(?:0[1-9]|1[0-6])$", ErrorMessage = "GhanaCard must follow the format: GHA-xxxxxxxxxx-01 to 16")] string? GhanaCard,

    [Required, StringLength(30, MinimumLength = 3)] string Surname,

    [Required, StringLength(50, MinimumLength = 3)] string OtherNames,

    [Required] DateTime DateOfBirth,

    [Required, StringLength(6, MinimumLength = 4), AllowedValues("Male", "Female")] string Sex,

    [Required, DataType(DataType.PhoneNumber)] string PhoneNumber);

    public record ListPatientsDto(
        Guid PatientsID,
        Guid SchemesID,
        short Age,
        string Gender,
        string FullName,
        string Scheme,
        string HospitalID,
        string CardID,
        DateTime ExpiryDate,
        string VisitType,
        DateTime AttendanceDate,
        Guid PatientSchemesID
        );
}
