namespace EvacuationSimulationAPI.Models
{
    public class Room
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int ExitId { get; set; }
        public Exit Exit { get; set; }
    }
}
