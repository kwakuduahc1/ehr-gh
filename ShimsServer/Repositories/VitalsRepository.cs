using Dapper;
using ShimsServer.Models.OPD;

namespace ShimsServer.Repositories
{
    public interface IVitalsRepository
    {
        public Task<VitalsummaryDto> GetVitalsForPatient(Guid patientId, CancellationToken cancellationToken);

        public Task<int> AddVitals(AddVitalsDto dto, Guid vid, string userName, CancellationToken cancellationToken);
    }
    public class VitalsRepository(IConnection connection) : IVitalsRepository
    {
        public async Task<int> AddVitals(AddVitalsDto dto, Guid vid, string userName, CancellationToken cancellationToken)
        {
            const string sql =
                """
                    INSERT INTO Vitals (VitalsID, PatientAttendancesID, DateSeen, Temperature, Weight, Pulse, Systol, Diastol, Respiration, SPO2, Complaints, Notes, UserName, PatientsID)
                    VALUES (@VitalsID, @PatientAttendancesID, now(), @Temperature, @Weight, @Pulse, @Systol, @Diastol, @Respiration, @SPO2, @Complaints, @Notes, @UserName, @PatientsID);
                """;
            using var con = await connection.ConnectionAsync(cancellationToken);
            using var transaction = await con.BeginTransactionAsync(cancellationToken);
            var res = await con.ExecuteAsync(sql, new
            {
                VitalsID = vid,
                dto.PatientAttendancesID,
                dto.Temperature,
                dto.Weight,
                dto.Pulse,
                dto.Systol,
                dto.Diastol,
                dto.Respiration,
                dto.SPO2,
                dto.Complaints,
                dto.Notes,
                UserName = userName,
                dto.PatientsID
            }, transaction: transaction);
            await transaction.CommitAsync(cancellationToken);
            return res;
        }

        public async Task<VitalsummaryDto> GetVitalsForPatient(Guid id, CancellationToken cancellationToken)
        {
            const string sql =
                """
                SELECT v.VitalsID, v.PatientAttendancesID, v.DateSeen, v.Temperature, v.Weight, v.Pulse, v.Systol, v.Diastol, v.Respiration, v.SPO2, v.Complaints, v.Notes, v.UserName
                FROM Vitals v
                WHERE v.PatientsID = @id
                ORDER BY v.DateSeen DESC
                LIMIT 30;
                SELECT patientsID, patientattendancesid, hospitalid, fullname, sex, age, visittype
                FROM vw_patients
                WHERE patientsid = @id;
                """;
            using var con = await connection.ConnectionAsync(cancellationToken);
            var result = await con.QueryMultipleAsync(sql, new { id });
            var vitals = await result.ReadAsync<VitalsDTO>();
            Console.WriteLine($"Vitals count: {vitals.Count()}");
            var patient = await result.ReadFirstOrDefaultAsync<LitePatientDto>();
            Console.WriteLine($"Patient: {patient?.FullName}");
            await con.CloseAsync();
            return new VitalsummaryDto(vitals, patient!);
        }
    }
}
