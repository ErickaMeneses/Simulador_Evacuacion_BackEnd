namespace EvacuationSimulationAPI.Models
{
    public class EvacuationResult
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public string PersonName { get; set; }
        public int ExitId { get; set; }
        public string ExitLocation { get; set; }
        public double TimeToEvacuate { get; set; }
        public DateTime SimulationDate { get; set; }
    }
}
