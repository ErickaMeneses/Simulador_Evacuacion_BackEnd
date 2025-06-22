namespace EvacuationSimulationAPI.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public double Speed { get; set; } // Metros por segundo o similar
        public int RoomId { get; set; }
        public Room Room { get; set; }
    }
}
