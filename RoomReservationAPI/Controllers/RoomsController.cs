using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomReservationAPI.Data;
using RoomReservationAPI.Models;
using RoomReservationAPI.DTOs;

namespace RoomReservationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public RoomsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/rooms
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoomDto>>> GetRooms()
        {
            var rooms = await _context.Rooms
                .Select(r => new RoomDto
                {
                    Id = r.Id,
                    Name = r.Name,
                    Location = r.Location,
                    Capacity = r.Capacity,
                    Description = r.Description
                })
                .ToListAsync();

            return rooms;
        }

        // GET: api/rooms/5
        [HttpGet("{id}")]
        public async Task<ActionResult<RoomDto>> GetRoom(int id)
        {
            var room = await _context.Rooms.FindAsync(id);

            if (room == null)
            {
                return NotFound();
            }

            var roomDto = new RoomDto
            {
                Id = room.Id,
                Name = room.Name,
                Location = room.Location,
                Capacity = room.Capacity,
                Description = room.Description
            };

            return roomDto;
        }

        // POST: api/rooms
        [HttpPost]
        public async Task<ActionResult<RoomDto>> PostRoom(CreateRoomDto roomRequest)
        {
            var room = new Room
            {
                Name = roomRequest.Name,
                Location = roomRequest.Location,
                Capacity = roomRequest.Capacity,
                Description = roomRequest.Description
            };

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();

            var roomDto = new RoomDto
            {
                Id = room.Id,
                Name = room.Name,
                Location = room.Location,
                Capacity = room.Capacity,
                Description = room.Description
            };

            return CreatedAtAction("GetRoom", new { id = roomDto.Id }, roomDto);
        }

        // PUT: api/rooms/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutRoom(int id, CreateRoomDto roomRequest)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            room.Name = roomRequest.Name;
            room.Location = roomRequest.Location;
            room.Capacity = roomRequest.Capacity;
            room.Description = roomRequest.Description;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return NoContent();
        }

        // DELETE: api/rooms/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return NotFound();
            }

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}