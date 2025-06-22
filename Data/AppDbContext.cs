using EvacuationSimulationAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EvacuationSimulationAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Exit> Exits { get; set; }
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Person> People { get; set; }
        public DbSet<EvacuationResult> EvacuationResults { get; set; }

    }
}
