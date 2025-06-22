namespace EvacuationSimulationAPI.Models
{
    public class Exit
    {
        public int Id { get; set; }
        public string Location { get; set; }
        public int Capacity { get; set; }// Personas que puede evacuar por unidad de tiempo
    }
}
