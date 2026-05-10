using Dapper;
using ShimsServer.Models.ConsultingRoom;
using ShimsServer.Models.Drugs;
using ShimsServer.Models.Investigations;
using ShimsServer.Models.Services;

namespace ShimsServer.Repositories
{
    public interface IConsultationsRepository
    {
        #region signs-and-symptoms

        Task<int> AddConsultation(AddPatientConsultationDto dto, (Guid id, string user) Info, CancellationToken token);

        Task<int> DeleteConsultation(Guid id, CancellationToken token);

        Task<IEnumerable<PatientConsultationDto>> GetConsultations(Guid id, CancellationToken token);
        #endregion

        #region investigation-requests

        Task<int> AddInvestigationRequest(AddInvestigationRequestDto request, (Guid id, string user) Info, CancellationToken token);

        Task<int> DeleteRequest(Guid id, CancellationToken token);

        Task<IEnumerable<PatientAttendanceInvestigations>> GetInvestigationRequests(Guid id, CancellationToken cancellationToken);
        #endregion

        #region prescriptions
        Task<int> AddPrescription(AddDrugRequestDto request, (Guid id, string user) Info, CancellationToken token);

        Task<int> DeletePrescription(Guid id, CancellationToken token);

        Task<int> UpdatePrescription(EditDrugsRequestDto request, (Guid id, string user) Info, CancellationToken token);
        #endregion

        #region services-requests

        Task<int> AddServvicesRequest(AddServiceRequestDto request, (Guid id, string user) Info, CancellationToken token);

        Task<int> DeleteServvicesRequest(Guid id, CancellationToken token);

        #endregion

        #region outcomes
        public Task<int> AddOutcome(AddPatientOutcomeDto dto, (Guid id, string user) Info, CancellationToken token);

        public Task<int> DeleteOutcome(Guid id, CancellationToken token);

        #endregion
    }

    public class ConsultationsRepository(IConnection connection) : IConsultationsRepository
    {
        public async Task<int> AddConsultation(AddPatientConsultationDto dto, (Guid id, string user) info, CancellationToken token)
        {
            const string sql = """
                INSERT INTO patientconsultations(
                    patientconsultationid, patientsattendancesid, complaints, odq, avpu, dateadded, username)
                VALUES (@id, @patientAttendanceId, @complaints, @odq, @avpu, now(), @userName)
                """;

            using var con = await connection.ConnectionAsync(token);
            return await con.ExecuteAsync(sql, new
            {
                info.id,
                dto.PatientsAttendancesID,
                dto.Complaints,
                dto.ODQ,
                dto.AVPU,
                userName = info.user
            });
        }

        public async Task<IEnumerable<PatientAttendanceInvestigations>> GetPatientAttendanceInvestigationsAsync(Guid id, CancellationToken token)
        {
            const string sql = """
                SELECT ir.investigationsrequestsid, ir.investigationsid, ir.daterequested, i.investigation
                FROM investigationsrequests ir
                INNER join investigations i ON i.investigationsid = ir.investigationsid
                where ir.patientattendancesid = @id
                """;
            using var con = await connection.ConnectionAsync(token);
            return await con.QueryAsync<PatientAttendanceInvestigations>(sql, new {id});
        }

        public async Task<int> AddInvestigationRequest(AddInvestigationRequestDto request, (Guid id, string user) Info, CancellationToken token)
        {
            const string sql = """
                INSERT INTO investigationsrequests(
                    investigationsrequestsid, patientsattendancesid, investigationsid, daterequested, username)
                VALUES (@id, @patientsAttendancesId, @investigationsId, now(), @userName)
                """;

            using var con = await connection.ConnectionAsync(token);
            return await con.ExecuteAsync(sql, new
            {
                Info.id,
                request.PatientsAttendancesID,
                request.SchemeInvestigationsID,
                userName = Info.user
            });
        }

        public async Task<int> AddOutcome(AddPatientOutcomeDto dto, (Guid id, string user) Info, CancellationToken token)
        {
            const string sql = """
                INSERT INTO patientoutcomes(
                    patientsattendancesid, patientid, outcome, notes, outcomedate, username)
                VALUES (@patientsAttendancesId, @patientId, @outcome, @notes, now(), @userName)
                """;

            using var con = await connection.ConnectionAsync(token);
            return await con.ExecuteAsync(sql, new
            {
                dto.PatientsAttendancesID,
                dto.PatientID,
                dto.Outcome,
                dto.Notes,
                userName = Info.user
            });
        }

        public async Task<int> DeleteOutcome(Guid id, CancellationToken token)
        {
            const string sql = """
                DELETE FROM patientoutcomes WHERE patientsattendancesid = @id
                """;

            using var con = await connection.ConnectionAsync(token);
            return await con.ExecuteAsync(sql, new { id });
        }

        public async Task<int> AddPrescription(AddDrugRequestDto request, (Guid id, string user) Info, CancellationToken token)
        {
            const string sql = """
                INSERT INTO drugsrequests(
                    drugsrequestsid, patientsid, schemedrugsid, frequency, days, quantityrequested, daterequested, username, ispaid, isdispensed)
                VALUES (@id, @patientsId, @schemeDrugsId, @frequency, @days, @quantityRequested, now(), @userName, false, false)
                """;

            using var con = await connection.ConnectionAsync(token);
            return await con.ExecuteAsync(sql, new
            {
                Info.id,
                request.PatientsID,
                request.SchemeDrugsID,
                request.Frequency,
                request.Days,
                request.QuantityRequested,
                userName = Info.user
            });
        }

        public async Task<int> AddServvicesRequest(AddServiceRequestDto request, (Guid id, string user) Info, CancellationToken token)
        {
            const string sql = """
                INSERT INTO servicerequests(
                    servicerequestid, patientsattendancesid, schemeservicesid, frequency, daterequested, username)
                VALUES (@id, @patientsAttendancesId, @schemeServicesId, @frequency, now(), @userName)
                """;

            using var con = await connection.ConnectionAsync(token);
            return await con.ExecuteAsync(sql, new
            {
                Info.id,
                request.PatientsAttendancesID,
                request.SchemeServicesID,
                request.Frequency,
                userName = Info.user
            });
        }

        public async Task<int> DeleteConsultation(Guid id, CancellationToken token)
        {
            const string sql = """
                DELETE FROM patientconsultations WHERE patientconsultationid = @id
                """;

            using var con = await connection.ConnectionAsync(token);
            return await con.ExecuteAsync(sql, new { id });
        }

        public async Task<int> DeletePrescription(Guid id, CancellationToken token)
        {
            const string sql = """
                DELETE FROM drugsrequests WHERE drugsrequestsid = @id
                """;

            using var con = await connection.ConnectionAsync(token);
            return await con.ExecuteAsync(sql, new { id });
        }

        public async Task<int> DeleteRequest(Guid id, CancellationToken token)
        {
            const string sql = """
                DELETE FROM investigationsrequests WHERE labrequestsid = @id
                """;

            using var con = await connection.ConnectionAsync(token);
            return await con.ExecuteAsync(sql, new { id });
        }

        public async Task<int> DeleteServvicesRequest(Guid id, CancellationToken token)
        {
            const string sql = """
                DELETE FROM servicerequests WHERE servicerequestid = @id
                """;

            using var con = await connection.ConnectionAsync(token);
            return await con.ExecuteAsync(sql, new { id });
        }

        public async Task<IEnumerable<PatientConsultationDto>> GetConsultations(Guid id, CancellationToken token)
        {
            const string sql = """
                SELECT patientsattendancesid, complaints, odq, avpu
                FROM patientconsultations
                WHERE patientsattendancesid = @id
                ORDER BY dateadded DESC
                LIMIT 15
                """;

            using var con = await connection.ConnectionAsync(token);
            return await con.QueryAsync<PatientConsultationDto>(sql, new { id });
        }

        public async Task<int> UpdatePrescription(EditDrugsRequestDto request, (Guid id, string user) Info, CancellationToken token)
        {
            const string sql = """
                UPDATE drugsrequests 
                SET frequency = @frequency, 
                    days = @days, 
                    quantityrequested = @quantityRequested
                WHERE drugsrequestsid = @drugsRequestsId
                """;

            using var con = await connection.ConnectionAsync(token);
            return await con.ExecuteAsync(sql, new
            {
                request.Frequency,
                request.Days,
                request.QuantityRequested,
                request.DrugsRequestsID
            });
        }

        public Task<IEnumerable<PatientAttendanceInvestigations>> GetInvestigationRequests(Guid id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
