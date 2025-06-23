//using EvacuationSimulationAPI.Data;
//using EvacuationSimulationAPI.Models;
//using Microsoft.EntityFrameworkCore;

//namespace EvacuationSimulationAPI.Services
//{
//    public class EvacuationService : IEvacuationService
//    {
//        private readonly AppDbContext _context;

//        public EvacuationService(AppDbContext context)
//        {
//            _context = context;
//        }

//        public async Task<int> SimulateEvacuationAsync()
//        {
//            var exits = await _context.Exits.ToListAsync();
//            var rooms = await _context.Rooms.ToListAsync();
//            var people = await _context.People.ToListAsync();

//            var evacuationQueue = new Dictionary<int, Queue<Person>>();

//            foreach (var exit in exits)
//            {
//                var roomsForExit = rooms.Where(r => r.ExitId == exit.Id)
//                                        .OrderBy(r => r.Id) // Cambiá a distancia si la agregás
//                                        .ToList();

//                var peopleQueue = new Queue<Person>();
//                foreach (var room in roomsForExit)
//                {
//                    var peopleInRoom = people.Where(p => p.RoomId == room.Id).ToList();
//                    foreach (var person in peopleInRoom)
//                    {
//                        peopleQueue.Enqueue(person);
//                    }
//                }
//                evacuationQueue.Add(exit.Id, peopleQueue);
//            }

//            var results = new List<EvacuationResult>();
//            double timeElapsed = 0;
//            bool evacuationCompleted = false;

//            while (!evacuationCompleted)
//            {
//                evacuationCompleted = true;

//                foreach (var exit in exits)
//                {
//                    if (evacuationQueue[exit.Id].Count > 0)
//                    {
//                        evacuationCompleted = false;

//                        for (int i = 0; i < exit.Capacity; i++)
//                        {
//                            if (evacuationQueue[exit.Id].Count > 0)
//                            {
//                                //
//                                var person = evacuationQueue[exit.Id].Dequeue();
//                                double timeToEvacuate = 1 / person.Speed; // Ajustá la fórmula a tu criterio

//                                results.Add(new EvacuationResult
//                                {
//                                    PersonId = person.Id,
//                                    PersonName = person.Name,
//                                    ExitId = exit.Id,
//                                    ExitLocation = exit.Location,
//                                    TimeToEvacuate = timeElapsed + timeToEvacuate,
//                                    SimulationDate = DateTime.Now
//                                });
//                            }
//                        }
//                    }
//                }

//                timeElapsed += 1;
//            }

//            await _context.EvacuationResults.AddRangeAsync(results);
//            await _context.SaveChangesAsync();

//            return results.Count;
//        }
//    }
//}


// Agrega estos using si no están

using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using EvacuationSimulationAPI.Models;
using EvacuationSimulationAPI.Data;
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
                var roomsForExit = rooms.Where(r => r.ExitId == exit.Id).ToList();
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
                                double timeToEvacuate = 1 / person.Speed;

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

            // CALCULAR M/M/m
            double totalPeople = people.Count;
            double totalTime = timeElapsed;

            double lambda = totalPeople / totalTime; // tasa de llegada
            double mu = people.Average(p => p.Speed); // tasa de servicio individual promedio
            int m = exits.Sum(e => e.Capacity); // total de servidores paralelos

            double rho = lambda / (m * mu);

            // Probabilidad de cero clientes en el sistema
            double sum = 0;
            for (int n = 0; n < m; n++)
            {
                sum += Math.Pow(lambda / mu, n) / Factorial(n);
            }
            double lastTerm = Math.Pow(lambda / mu, m) / (Factorial(m) * (1 - rho));
            double P0 = 1 / (sum + lastTerm);

            double Lq = P0 * (Math.Pow(lambda / mu, m) * rho) / (Factorial(m) * Math.Pow(1 - rho, 2));
            double Wq = Lq / lambda;
            double W = Wq + 1 / mu;
            double L = lambda * W;

            // Mostrar en consola para verificar
            Console.WriteLine($"--- Teoría de Colas M/M/m ---");
            Console.WriteLine($"λ = {lambda:F3} personas/s");
            Console.WriteLine($"μ = {mu:F3} personas/s");
            Console.WriteLine($"m = {m} servidores");
            Console.WriteLine($"ρ = {rho:F3}");
            Console.WriteLine($"P₀ = {P0:F3}");
            Console.WriteLine($"Lq = {Lq:F3} personas");
            Console.WriteLine($"Wq = {Wq:F3} s");
            Console.WriteLine($"W = {W:F3} s");
            Console.WriteLine($"L = {L:F3} personas");

            // Guardar resultado teórico en base de datos
            var theoreticalResult = new TheoreticalResult
            {
                Lambda = lambda,
                Mu = mu,
                m = m,
                Rho = rho,
                P0 = P0,
                Lq = Lq,
                Wq = Wq,
                W = W,
                L = L,
                SimulationDate = DateTime.Now
            };
            await _context.TheoreticalResults.AddAsync(theoreticalResult);
            await _context.SaveChangesAsync();



            return results.Count;
        }

        // Factorial helper
        private double Factorial(int n)
        {
            if (n == 0) return 1;
            double result = 1;
            for (int i = 1; i <= n; i++) result *= i;
            return result;
        }


    }
}

