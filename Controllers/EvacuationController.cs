using EvacuationSimulationAPI.Data;
using EvacuationSimulationAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EvacuationSimulationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EvacuationController : ControllerBase
    {
        private readonly IEvacuationService _evacuationService;
        private readonly AppDbContext _context;

        public EvacuationController(IEvacuationService evacuationService, AppDbContext context)
        {
            _evacuationService = evacuationService;
            _context = context;
        }

        [HttpPost("start")]
        public async Task<IActionResult> StartEvacuation()
        {
            // BORRA resultados previos
            _context.EvacuationResults.RemoveRange(_context.EvacuationResults);
            await _context.SaveChangesAsync();

            // Simula y guarda nuevos resultados
            var totalEvacuated = await _evacuationService.SimulateEvacuationAsync();
            return Ok(new { Message = "Evacuación completada", TotalPeopleEvacuated = totalEvacuated });
        }

        [HttpGet("results")]
        public async Task<IActionResult> GetResults()
        {
            var results = await _context.EvacuationResults.OrderBy(r => r.TimeToEvacuate).ToListAsync();
            return Ok(results);
        }
    }
}
