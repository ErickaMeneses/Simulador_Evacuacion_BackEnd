namespace EvacuationSimulationAPI.Models
{
    public class TheoreticalResult
    {
        public int Id { get; set; }
        public double Lambda { get; set; }
        public double Mu { get; set; }
        public int m { get; set; }
        public double Rho { get; set; }
        public double P0 { get; set; }
        public double Lq { get; set; }
        public double Wq { get; set; }
        public double W { get; set; }
        public double L { get; set; }
        public DateTime SimulationDate { get; set; }
    }
}
