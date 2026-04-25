// CORRECTED VERSION - AddPatientAsync Method
// Recommended approach for .NET 10 with .NETCore best practices

using Dapper;
using System.ComponentModel.DataAnnotations;

namespace ShimsServer.Repositories
{
    public partial class RegistrationRepository
    {
        /// <summary>
        /// Adds a new patient with their schemes and initial attendance record.
        /// Uses separate parameterized queries for better maintainability and safety.
        /// Thread-safe and transaction-protected.
        /// </summary>
        public async Task AddPatientAsync(
            AddPatientDto dto, 
            (Guid PatientsID, Guid PatientAttendancesID, string UserName) ids, 
            CancellationToken cancellationToken = default)
        {
            if (dto == null)
                throw new ArgumentNullException(nameof(dto));

            if (dto.Schemes == null || dto.Schemes.Length == 0)
                throw new ArgumentException("Patient must have at least one scheme", nameof(dto));

            try
            {
                using var con = await connection.ConnectionAsync(cancellationToken);
                using var transaction = await con.BeginTransactionAsync(cancellationToken);

                try
                {
                    // Step 1: Insert Patient and retrieve generated HospitalID
                    var hospitalId = await InsertPatientAsync(
                        con, transaction, dto, ids, cancellationToken);

                    // Step 2: Insert Patient Schemes (multiple records)
                    await InsertPatientSchemesAsync(
                        con, transaction, ids.PatientsID, dto.Schemes, dto.CardID, 
                        dto.ExpiryDate, ids.UserName, cancellationToken);

                    // Step 3: Insert Initial Attendance Record
                    await InsertPatientAttendanceAsync(
                        con, transaction, ids.PatientAttendancesID, 
                        ids.PatientsID, ids.UserName, cancellationToken);

                    await transaction.CommitAsync(cancellationToken);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(cancellationToken);
                    throw new InvalidOperationException(
                        "Failed to add patient. Transaction rolled back.", ex);
                }
            }
            catch (Exception ex)
            {
                // Log exception here if using ILogger
                throw;
            }
        }

        /// <summary>
        /// Inserts a patient record and returns the generated HospitalID
        /// </summary>
        private async Task<string> InsertPatientAsync(
            IDbConnection con,
            IDbTransaction transaction,
            AddPatientDto dto,
            (Guid PatientsID, Guid PatientAttendancesID, string UserName) ids,
            CancellationToken cancellationToken)
        {
            const string sql = """
                INSERT INTO Patients (
                    PatientID, 
                    Surname, 
                    OtherNames, 
                    DateOfBirth, 
                    GhanaCard, 
                    Sex, 
                    PhoneNumber, 
                    UserName, 
                    HospitalID
                )
                VALUES (
                    @PatientID, 
                    @Surname, 
                    @OtherNames, 
                    @DateOfBirth, 
                    @GhanaCard, 
                    @Sex, 
                    @PhoneNumber, 
                    @UserName, 
                    generate_hospital_id()
                )
                RETURNING HospitalID;
                """;

            var result = await con.ExecuteScalarAsync<string>(
                sql,
                new
                {
                    PatientID = ids.PatientsID,
                    dto.Surname,
                    dto.OtherNames,
                    dto.DateOfBirth,
                    dto.GhanaCard,
                    dto.Sex,
                    dto.PhoneNumber,
                    UserName = ids.UserName
                },
                transaction);

            if (string.IsNullOrEmpty(result))
                throw new InvalidOperationException("Failed to generate HospitalID");

            return result;
        }

        /// <summary>
        /// Inserts multiple PatientSchemes records for a patient
        /// </summary>
        private async Task InsertPatientSchemesAsync(
            IDbConnection con,
            IDbTransaction transaction,
            Guid patientId,
            Guid[] schemes,
            string cardId,
            DateTime expiryDate,
            string userName,
            CancellationToken cancellationToken)
        {
            if (schemes == null || schemes.Length == 0)
                return;

            const string sql = """
                INSERT INTO PatientSchemes (
                    PatientSchemesID, 
                    PatientsID, 
                    SchemesID, 
                    CardID, 
                    ExpiryDate, 
                    LastUpdateDate, 
                    UserName, 
                    IsActive
                )
                VALUES (
                    @PatientSchemesID, 
                    @PatientID, 
                    @SchemesID, 
                    @CardID, 
                    @ExpiryDate, 
                    NOW(), 
                    @UserName, 
                    true
                );
                """;

            foreach (var schemeId in schemes)
            {
                await con.ExecuteAsync(
                    sql,
                    new
                    {
                        PatientSchemesID = Guid.CreateVersion7(),
                        PatientID = patientId,
                        SchemesID = schemeId,
                        CardID = cardId,
                        ExpiryDate = expiryDate,
                        UserName = userName
                    },
                    transaction);
            }
        }

        /// <summary>
        /// Inserts the initial PatientAttendance record for a new patient
        /// </summary>
        private async Task InsertPatientAttendanceAsync(
            IDbConnection con,
            IDbTransaction transaction,
            Guid attendanceId,
            Guid patientId,
            string userName,
            CancellationToken cancellationToken)
        {
            const string sql = """
                INSERT INTO PatientAttendances (
                    PatientAttendancesID, 
                    PatientsID, 
                    VisitType, 
                    UserName, 
                    DateSeen, 
                    IsActive
                )
                VALUES (
                    @PatientAttendancesID, 
                    @PatientID, 
                    @VisitType, 
                    @UserName, 
                    NOW(), 
                    true
                );
                """;

            var rowsAffected = await con.ExecuteAsync(
                sql,
                new
                {
                    PatientAttendancesID = attendanceId,
                    PatientID = patientId,
                    VisitType = "Acute",
                    UserName = userName
                },
                transaction);

            if (rowsAffected == 0)
                throw new InvalidOperationException("Failed to insert PatientAttendance record");
        }
    }
}

// ============================================
// ALTERNATIVE: Batch Insert with UNNEST
// ============================================
// Uncomment if you prefer single database round-trip

/*
public async Task AddPatientAsync(
    AddPatientDto dto, 
    (Guid PatientsID, Guid PatientAttendancesID, string UserName) ids, 
    CancellationToken cancellationToken = default)
{
    if (dto?.Schemes == null || dto.Schemes.Length == 0)
        throw new ArgumentException("Patient must have at least one scheme", nameof(dto));

    try
    {
        using var con = await connection.ConnectionAsync(cancellationToken);
        using var transaction = await con.BeginTransactionAsync(cancellationToken);

        try
        {
            const string sql = """
                -- Insert Patient
                INSERT INTO Patients (
                    PatientID, Surname, OtherNames, DateOfBirth, GhanaCard, 
                    Sex, PhoneNumber, UserName, HospitalID
                )
                VALUES (
                    @PatientID, @Surname, @OtherNames, @DateOfBirth, @GhanaCard, 
                    @Sex, @PhoneNumber, @UserName, generate_hospital_id()
                )
                RETURNING HospitalID;

                -- Insert Patient Schemes (bulk)
                INSERT INTO PatientSchemes (
                    PatientSchemesID, PatientsID, SchemesID, CardID, 
                    ExpiryDate, LastUpdateDate, UserName, IsActive
                )
                SELECT 
                    gen_random_uuid(), 
                    @PatientID, 
                    scheme_id, 
                    @CardID, 
                    @ExpiryDate, 
                    NOW(), 
                    @UserName, 
                    true
                FROM UNNEST(@SchemeIds) AS schemes(scheme_id);

                -- Insert Patient Attendance
                INSERT INTO PatientAttendances (
                    PatientAttendancesID, PatientsID, VisitType, 
                    UserName, DateSeen, IsActive
                )
                VALUES (
                    @AttendanceID, @PatientID, @VisitType, 
                    @UserName, NOW(), true
                );
                """;

            using var reader = await con.QueryMultipleAsync(
                sql,
                new
                {
                    PatientID = ids.PatientsID,
                    dto.Surname,
                    dto.OtherNames,
                    dto.DateOfBirth,
                    dto.GhanaCard,
                    dto.Sex,
                    dto.PhoneNumber,
                    UserName = ids.UserName,
                    dto.CardID,
                    dto.ExpiryDate,
                    SchemeIds = dto.Schemes,
                    AttendanceID = ids.PatientAttendancesID,
                    VisitType = "Acute"
                },
                transaction);

            // Consume all result sets
            var hospitalId = await reader.ReadFirstOrDefaultAsync<string>();
            if (string.IsNullOrEmpty(hospitalId))
                throw new InvalidOperationException("Failed to generate HospitalID");

            // Schemes insertion result (no data returned)
            await reader.ReadAsync();

            // Attendance insertion result (no data returned)
            await reader.ReadAsync();

            await transaction.CommitAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(cancellationToken);
            throw new InvalidOperationException("Failed to add patient. Transaction rolled back.", ex);
        }
    }
    catch (Exception ex)
    {
        throw;
    }
}
*/
