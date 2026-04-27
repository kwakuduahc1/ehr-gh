using Dapper;
using System.ComponentModel.DataAnnotations;

namespace ShimsServer.Repositories
{
    public interface IRegistrationRepository
    {
        Task<string> AddPatientAsync(AddPatientDto dto, (Guid PatientsID, Guid PatientAttendancesID, Guid[] PatientSchemesID, string UserName) ids, CancellationToken cancellationToken = default);

        Task<int> EditPatientAsync(EditPatientDto dto, string UserName, CancellationToken cancellationToken = default);

        Task DeletePatientAsync(Guid id, CancellationToken cancellationToken = default);

        Task<IEnumerable<PatientDetailsDto>> GetPatientsAsync(CancellationToken cancellationToken = default);

        Task<bool> PatientExists(Guid id, CancellationToken cancellationToken = default);

        Task<PatientDetailsDto?> GetPatientByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IEnumerable<PatientDetailsDto>> SearchPatientsAsync(string search, CancellationToken cancellationToken = default);

        Task<PatientDetailsDto?> GetPatientDetailsByIdAsync(Guid id, CancellationToken cancellationToken = default);
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

                    INSERT INTO PatientSchemes (PatientSchemesID, PatientsID, IsActive, SchemesID, CardID, ExpiryDate, LastUpdateDate, UserName)
                    VALUES (
                        uuidv7(), 
                        @PatientsID, 
                        true, 
                        (SELECT schemesid from schemes
                        WHERE schemename = 'Fee Paying'), 
                        null, null, now(), @UserName);

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

        public async Task<PatientDetailsDto?> GetPatientByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            const string sql =
                """
                    SELECT PatientsID, schemesid, age, sex, fullname, hospitalid, cardid, expirydate, dateseen, patientschemesid, visittype, phonenumber, ghanacard, dateofbirth, schemename, coverage
                    FROM vw_patients
                    WHERE PatientsID = @id;
                """;
            using var con = await connection.ConnectionAsync(cancellationToken);
            var res = await con.QueryAsync<PatientDetails>(sql, new { id });
            return res
                .GroupBy(p => new
                {
                    p.FullName,
                    p.HospitalID,
                    p.PhoneNumber,
                    p.GhanaCard,
                    p.VisitType,
                    p.PatientAttendancesID,
                    p.DateSeen,
                    p.PatientsID,
                    p.Age,
                    p.Sex,
                    p.DateOfBirth
                }, (k, v) => new PatientDetailsDto(
                    k.PatientsID,
                    k.HospitalID,
                    k.FullName,
                    k.Sex,
                    k.DateOfBirth,
                    k.Age,
                    k.PhoneNumber,
                    k.GhanaCard,
                    k.VisitType,
                    k.PatientAttendancesID,
                    k.DateSeen,
                    v.Select(s => new InsuranceDetails(
                        s.SchemesID,
                        s.CardID,
                        s.ExpiryDate,
                        s.PatientSchemesID,
                        s.SchemeName,
                        s.Coverage
                    ))
                )).FirstOrDefault();
        }

        public async Task<IEnumerable<PatientDetailsDto>> GetPatientsAsync(CancellationToken cancellationToken = default)
        {
            const string sql =
                """
                    SELECT PatientsID, schemesid, age, sex, fullname, hospitalid, cardid, expirydate, dateseen, patientschemesid, visittype, phonenumber, ghanacard, dateofbirth, schemename, coverage
                    FROM vw_patients;
                """;
            using var con = await connection.ConnectionAsync(cancellationToken);
            var res = await con.QueryAsync<PatientDetails>(sql);
            return res
                .GroupBy(p => new
                {
                    p.FullName,
                    p.HospitalID,
                    p.PhoneNumber,
                    p.GhanaCard,
                    p.VisitType,
                    p.PatientAttendancesID,
                    p.DateSeen,
                    p.PatientsID,
                    p.Age,
                    p.Sex,
                    p.DateOfBirth
                }, (k, v) => new PatientDetailsDto(
                    k.PatientsID,
                    k.HospitalID,
                    k.FullName,
                    k.Sex,
                    k.DateOfBirth,
                    k.Age,
                    k.PhoneNumber,
                    k.GhanaCard,
                    k.VisitType,
                    k.PatientAttendancesID,
                    k.DateSeen,
                    v.Select(s => new InsuranceDetails(
                        s.SchemesID,
                        s.CardID,
                        s.ExpiryDate,
                        s.PatientSchemesID,
                        s.SchemeName,
                        s.Coverage
                    ))
                ));
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

        public async Task<IEnumerable<PatientDetailsDto>> SearchPatientsAsync(string search, CancellationToken cancellationToken = default)
        {
            const string sql =
                """
                    SELECT PatientsID, schemesid, age, sex, fullname, hospitalid, cardid, expirydate, dateseen, patientschemesid, visittype, phonenumber, ghanacard, dateofbirth, schemename, coverage
                    FROM vw_patients
                    WHERE fullname ILIKE @search
                        OR cardid ILIKE @search
                        OR hospitalid ILIKE @search
                    ORDER BY dateseen DESC;
                """;
            using var con = await connection.ConnectionAsync(cancellationToken);
            var res = await con.QueryAsync<PatientDetails>(sql, new { search = $"%{search}%" });
            return res
                .GroupBy(p => new
                {
                    p.FullName,
                    p.HospitalID,
                    p.PhoneNumber,
                    p.GhanaCard,
                    p.VisitType,
                    p.PatientAttendancesID,
                    p.DateSeen,
                    p.PatientsID,
                    p.Age,
                    p.Sex,
                    p.DateOfBirth
                }, (k, v) => new PatientDetailsDto(
                    k.PatientsID,
                    k.HospitalID,
                    k.FullName,
                    k.Sex,
                    k.DateOfBirth,
                    k.Age,
                    k.PhoneNumber,
                    k.GhanaCard,
                    k.VisitType,
                    k.PatientAttendancesID,
                    k.DateSeen,
                    v.Select(s => new InsuranceDetails(
                        s.SchemesID,
                        s.CardID,
                        s.ExpiryDate,
                        s.PatientSchemesID,
                        s.SchemeName,
                        s.Coverage
                    ))
                ));
        }

        public async Task<PatientDetailsDto?> GetPatientDetailsByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            const string sql =
                """
                    SELECT PatientsID, hospitalid, fullname, sex, dateofbirth, age, phonenumber, ghanacard, visittype, patientattendancesid, dateseen, patientschemesid, cardid, expirydate, schemesid, schemename, coverage
                    FROM vw_patients
                    WHERE PatientsID = @id;
                """;
            using var con = await connection.ConnectionAsync(cancellationToken);
            var res = await con.QueryAsync<PatientDetails>(sql, new { id });
            return res
                .GroupBy(p => new
                {
                    p.FullName,
                    p.HospitalID,
                    p.PhoneNumber,
                    p.GhanaCard,
                    p.VisitType,
                    p.PatientAttendancesID,
                    p.DateSeen,
                    p.PatientsID,
                    p.Age,
                    p.Sex,
                    p.DateOfBirth
                }, (k, v) => new PatientDetailsDto(
                    k.PatientsID,
                    k.HospitalID,
                    k.FullName,
                    k.Sex,
                    k.DateOfBirth,
                    k.Age,
                    k.PhoneNumber,
                    k.GhanaCard,
                    k.VisitType,
                    k.PatientAttendancesID,
                    k.DateSeen,
                    v.Select(s => new InsuranceDetails(
                        s.SchemesID,
                        s.CardID,
                        s.ExpiryDate,
                        s.PatientSchemesID,
                        s.SchemeName,
                        s.Coverage
                    ))
                )).FirstOrDefault();
        }
    }

    public record InsuranceInformation(
        Guid SchemesID,
        [StringLength(30, MinimumLength = 5)] string? CardID,
        DateTime? ExpiryDate
        );

    public record InsuranceDetails(
         Guid SchemesID,
         [StringLength(30, MinimumLength = 5)] string? CardID,
         DateOnly? ExpiryDate,
         Guid PatientSchemesID,
         string SchemeName,
         string Coverage
         );

    public record AddPatientDto(
        [StringLength(30, MinimumLength = 3)] string Surname,

        [StringLength(50, MinimumLength = 3)] string OtherNames,

        DateTime DateOfBirth,
        [StringLength(17), RegularExpression(@"^GHA-\d{10}-(?:0[1-9]|1[0-6])$", ErrorMessage = "GhanaCard must follow the format: GHA-xxxxxxxxxx-01 to 16")] string? GhanaCard,

        [StringLength(6, MinimumLength = 4), AllowedValues("Male", "Female")] string Sex,

        [DataType(DataType.PhoneNumber)] string? PhoneNumber
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

    /**
 * (System.Guid patientsid, System.Guid schemesid, System.Int16 age, System.String sex, System.String fullname, System.String scheme, System.String hospitalid, System.String cardid, System.DateTime expirydate, System.DateTime dateseen, System.Guid patientschemesid)
 */
    public record ListPatientsDto(
        Guid PatientsID,
        Guid SchemesID,
        short Age,
        string Sex,
        string FullName,
        string Scheme,
        string HospitalID,
        string CardID,
        DateOnly? ExpiryDate,
        DateOnly DateSeen,
        Guid PatientSchemesID,
        string PhoneNumber,
        string VisitType
        );

    /*
     * (System.Guid patientsid, System.String hospitalid, System.String fullname, System.String sex, System.DateOnly dateofbirth, System.Int16 age, System.String phonenumber, System.String ghanacard, System.String visittype, System.Guid patientattendancesid, System.DateOnly dateseen, System.Guid patientschemesid, System.String cardid, System.DateOnly expirydate)
     * */

    record PatientDetails(
        Guid PatientsID,
        string HospitalID,
        string FullName,
        string Sex,
        DateOnly DateOfBirth,
        short Age,
        string PhoneNumber,
        string GhanaCard,
        string VisitType,
        Guid PatientAttendancesID,
        DateOnly DateSeen,
        Guid PatientSchemesID,
        string? CardID,
        DateOnly? ExpiryDate,
        Guid SchemesID,
        string SchemeName,
        string Coverage
        );

    public record PatientDetailsDto(
    Guid PatientsID,
    string HospitalID,
    string FullName,
    string Sex,
    DateOnly DateOfBirth,
    short Age,
    string PhoneNumber,
    string GhanaCard,
    string VisitType,
    Guid PatientAttendancesID,
    DateOnly DateSeen,
    IEnumerable<InsuranceDetails> Schemes
    );
}
