namespace ShimsServer.Models.ConsultingRoom
{
    public class PendingVm
    {
        public required string FullName { get; set; }

        public required string OPDNumber { get; set; }

        public required string Gender { get; set; }

        public int Age{ get; set; }

        public Guid PatientsID { get; set; }

        public double? Systolic { get; set; }

        public double? Diastolic { get; set; }

        public double? Weight { get; set; }

        public double? Pulse { get; set; }

        public double? Temperature { get; set; }

        public double? Respiration { get; set; }

        public string? History {  get; set; }
    }
}
