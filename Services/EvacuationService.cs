using EvacuationSimulationAPI.Data;
using EvacuationSimulationAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EvacuationSimulationAPI.Services
{
    public class EvacuationService : IEvacuationService
    {
        private readonly AppDbContext _context;

        public EvacuationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<int> SimulateEvacuationAsync()
        {
            var exits = await _context.Exits.ToListAsync();
            var rooms = await _context.Rooms.ToListAsync();
            var people = await _context.People.ToListAsync();

            var evacuationQueue = new Dictionary<int, Queue<Person>>();

            foreach (var exit in exits)
            {
                var roomsForExit = rooms.Where(r => r.ExitId == exit.Id)
                                        .OrderBy(r => r.Id) // Cambiá a distancia si la agregás
                                        .ToList();

                var peopleQueue = new Queue<Person>();
                foreach (var room in roomsForExit)
                {
                    var peopleInRoom = people.Where(p => p.RoomId == room.Id).ToList();
                    foreach (var person in peopleInRoom)
                    {
                        peopleQueue.Enqueue(person);
                    }
                }
                evacuationQueue.Add(exit.Id, peopleQueue);
            }

            var results = new List<EvacuationResult>();
            double timeElapsed = 0;
            bool evacuationCompleted = false;

            while (!evacuationCompleted)
            {
                evacuationCompleted = true;

                foreach (var exit in exits)
                {
                    if (evacuationQueue[exit.Id].Count > 0)
                    {
                        evacuationCompleted = false;

                        for (int i = 0; i < exit.Capacity; i++)
                        {
                            if (evacuationQueue[exit.Id].Count > 0)
                            {
                                var person = evacuationQueue[exit.Id].Dequeue();
                                double timeToEvacuate = 1 / person.Speed; // Ajustá la fórmula a tu criterio

                                results.Add(new EvacuationResult
                                {
                                    PersonId = person.Id,
                                    PersonName = person.Name,
                                    ExitId = exit.Id,
                                    ExitLocation = exit.Location,
                                    TimeToEvacuate = timeElapsed + timeToEvacuate,
                                    SimulationDate = DateTime.Now
                                });
                            }
                        }
                    }
                }

                timeElapsed += 1;
            }

            await _context.EvacuationResults.AddRangeAsync(results);
            await _context.SaveChangesAsync();

            return results.Count;
        }
    }
}
