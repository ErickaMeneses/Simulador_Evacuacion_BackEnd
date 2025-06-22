namespace EvacuationSimulationAPI.Services
{
    public interface IEvacuationService
    {
        Task<int> SimulateEvacuationAsync();
    }
}
