using EvacuationSimulationAPI.Data;
using EvacuationSimulationAPI.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EvacuationSimulationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ManagementController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ManagementController(AppDbContext context)
        {
            _context = context;
        }
        /*
        [HttpGet("exits")]
        public async Task<IActionResult> GetExits()
        {
            var exits = await _context.Exits.ToListAsync();
            return Ok(exits);
        }

        [HttpGet("rooms")]
        public async Task<IActionResult> GetRooms()
        {
            var rooms = await _context.Rooms.Include(r => r.Exit).ToListAsync();
            return Ok(rooms);
        }

        [HttpGet("people")]
        public async Task<IActionResult> GetPeople()
        {
            var people = await _context.People.Include(p => p.Room).ToListAsync();
            return Ok(people);
        }
        */

        // ---------- EXITS ----------

        [HttpGet("exits")]
        public async Task<IActionResult> GetExits()
        {
            var exits = await _context.Exits.ToListAsync();
            return Ok(exits);
        }

        [HttpGet("exits/{id}")]
        public async Task<IActionResult> GetExit(int id)
        {
            var exit = await _context.Exits.FindAsync(id);
            if (exit == null) return NotFound();
            return Ok(exit);
        }

        [HttpPost("exits")]
        public async Task<IActionResult> CreateExit([FromBody] Exit exit)
        {
            if (exit == null || string.IsNullOrWhiteSpace(exit.Location) || exit.Capacity <= 0)
                return BadRequest("Datos inválidos");

            _context.Exits.Add(exit);
            await _context.SaveChangesAsync();
            return Ok(exit);
        }

        [HttpPut("exits/{id}")]
        public async Task<IActionResult> UpdateExit(int id, [FromBody] Exit updatedExit)
        {
            var exit = await _context.Exits.FindAsync(id);
            if (exit == null) return NotFound();

            exit.Location = updatedExit.Location;
            exit.Capacity = updatedExit.Capacity;
            await _context.SaveChangesAsync();

            return Ok(exit);
        }

        [HttpDelete("exits/{id}")]
        public async Task<IActionResult> DeleteExit(int id)
        {
            var exit = await _context.Exits.FindAsync(id);
            if (exit == null) return NotFound();

            _context.Exits.Remove(exit);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ---------- ROOMS ----------

        [HttpGet("rooms")]
        public async Task<IActionResult> GetRooms()
        {
            var rooms = await _context.Rooms.Include(r => r.Exit).ToListAsync();
            return Ok(rooms);
        }

        [HttpGet("rooms/{id}")]
        public async Task<IActionResult> GetRoom(int id)
        {
            var room = await _context.Rooms.Include(r => r.Exit).FirstOrDefaultAsync(r => r.Id == id);
            if (room == null) return NotFound();
            return Ok(room);
        }

        [HttpPost("rooms")]
        public async Task<IActionResult> CreateRoom([FromBody] Room room)
        {
            var exit = await _context.Exits.FindAsync(room.ExitId);
            if (exit == null) return BadRequest("Exit asociado no existe.");

            if (string.IsNullOrWhiteSpace(room.Name))
                return BadRequest("Nombre requerido.");

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();
            return Ok(room);
        }

        [HttpPut("rooms/{id}")]
        public async Task<IActionResult> UpdateRoom(int id, [FromBody] Room updatedRoom)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null) return NotFound();

            var exit = await _context.Exits.FindAsync(updatedRoom.ExitId);
            if (exit == null) return BadRequest("Exit asociado no existe.");

            room.Name = updatedRoom.Name;
            room.ExitId = updatedRoom.ExitId;
            await _context.SaveChangesAsync();

            return Ok(room);
        }

        [HttpDelete("rooms/{id}")]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null) return NotFound();

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // ---------- PERSON ----------

        [HttpGet("people")]
        public async Task<IActionResult> GetPeople()
        {
            var people = await _context.People.Include(p => p.Room).ThenInclude(r => r.Exit).ToListAsync();
            return Ok(people);
        }

        [HttpGet("people/{id}")]
        public async Task<IActionResult> GetPerson(int id)
        {
            var person = await _context.People.Include(p => p.Room).ThenInclude(r => r.Exit).FirstOrDefaultAsync(p => p.Id == id);
            if (person == null) return NotFound();
            return Ok(person);
        }

        [HttpPost("people")]
        public async Task<IActionResult> CreatePerson([FromBody] Person person)
        {
            var room = await _context.Rooms.FindAsync(person.RoomId);
            if (room == null) return BadRequest("Room asociado no existe.");

            if (string.IsNullOrWhiteSpace(person.Name) || person.Speed <= 0)
                return BadRequest("Datos inválidos para la persona.");

            _context.People.Add(person);
            await _context.SaveChangesAsync();
            return Ok(person);
        }

        [HttpPut("people/{id}")]
        public async Task<IActionResult> UpdatePerson(int id, [FromBody] Person updatedPerson)
        {
            var person = await _context.People.FindAsync(id);
            if (person == null) return NotFound();

            var room = await _context.Rooms.FindAsync(updatedPerson.RoomId);
            if (room == null) return BadRequest("Room asociado no existe.");

            person.Name = updatedPerson.Name;
            person.Speed = updatedPerson.Speed;
            person.RoomId = updatedPerson.RoomId;

            await _context.SaveChangesAsync();
            return Ok(person);
        }

        [HttpDelete("people/{id}")]
        public async Task<IActionResult> DeletePerson(int id)
        {
            var person = await _context.People.FindAsync(id);
            if (person == null) return NotFound();

            _context.People.Remove(person);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
