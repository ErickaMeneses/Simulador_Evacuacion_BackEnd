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

        //Exit
        [HttpGet("exits")]
        public async Task<IActionResult> GetExits()
        {
            var exits = await _context.Exits.ToListAsync();
            return Ok(exits);
        }

        [HttpPost("exits")]
        public async Task<IActionResult> CreateExit([FromBody] Exit exit)
        {
            if (exit == null || string.IsNullOrWhiteSpace(exit.Location) || exit.Capacity <= 0)
            {
                return BadRequest("Datos inválidos.");
            }

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


        //Room
        [HttpGet("rooms")]
        public async Task<IActionResult> GetRooms()
        {
            var rooms = await _context.Rooms.Include(r => r.Exit).ToListAsync();
            return Ok(rooms);
        }

        [HttpPost("rooms")]
        public async Task<IActionResult> CreateRoom([FromBody] RoomCreateDto roomDto)
        {
            // Validar que exista el Exit
            var exit = await _context.Exits.FindAsync(roomDto.ExitId);
            if (exit == null)
            {
                return BadRequest($"Exit con ID {roomDto.ExitId} no existe.");
            }

            if (string.IsNullOrWhiteSpace(roomDto.Name))
            {
                return BadRequest("Nombre de la sala es obligatorio.");
            }

            var room = new Room
            {
                Name = roomDto.Name,
                ExitId = roomDto.ExitId
            };

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            return Ok(room);
        }

        [HttpPut("rooms/{id}")]
        public async Task<IActionResult> UpdateRoom(int id, [FromBody] Room updatedRoom)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null) return NotFound();

            // Validar nuevo Exit
            var exit = await _context.Exits.FindAsync(updatedRoom.ExitId);
            if (exit == null) return BadRequest("Exit no válido.");

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

        //Crear nueva Persona
        
        [HttpGet("people")]
        public async Task<IActionResult> GetPeople()
        {
            var people = await _context.People.Include(p => p.Room).ToListAsync();
            return Ok(people);
        }

        [HttpPost("people")]
        public async Task<IActionResult> CreatePerson([FromBody] PersonCreateDto personDto)
        {
            var room = await _context.Rooms.FindAsync(personDto.RoomId);
            if (room == null)
                return BadRequest($"Room con ID {personDto.RoomId} no existe.");

            var person = new Person
            {
                Name = personDto.Name,
                Speed = personDto.Speed,
                RoomId = personDto.RoomId
            };

            _context.People.Add(person);
            await _context.SaveChangesAsync();

            return Ok(person);
        }        

        [HttpPut("people/{id}")]
        public async Task<IActionResult> UpdatePerson(int id, [FromBody] PersonCreateDto updatedPerson)
        {
            var person = await _context.People.FindAsync(id);
            if (person == null) return NotFound();

            // Validar nuevo Room
            var room = await _context.Rooms.FindAsync(updatedPerson.RoomId);
            if (room == null) return BadRequest("Room no válido.");

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
