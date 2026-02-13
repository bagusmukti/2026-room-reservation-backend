using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomReservationAPI.Data;
using RoomReservationAPI.Models;
using RoomReservationAPI.DTOs;
using System.Security.Claims;

namespace RoomReservationAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ReservationsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/reservations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reservation>>> GetReservations()
        {
            return await _context.Reservations
                .Include(r => r.Room)
                .OrderByDescending(r => r.StartTime)
                .ToListAsync();
        }

        // GET: api/reservations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Reservation>> GetReservation(int id)
        {
            var reservation = await _context.Reservations.Include(r => r.Room).FirstOrDefaultAsync(r => r.Id == id);

            if (reservation == null)
            {
                return NotFound();
            }

            return reservation;
        }

        // POST: api/reservations
        [HttpPost]
        public async Task<ActionResult<Reservation>> PostReservation(CreateReservationDto request)
        {
            var room = await _context.Rooms.FindAsync(request.RoomId);
            if (room == null) return NotFound("Ruangan tidak ditemukan.");

            var startUtc = request.StartTime.ToUniversalTime();
            var endUtc = request.EndTime.ToUniversalTime();

            if (startUtc < DateTime.UtcNow) return BadRequest("Waktu mulai tidak boleh di masa lalu.");
            if (endUtc <= startUtc) return BadRequest("Waktu selesai harus lebih besar dari waktu mulai.");

            // Validasi Bentrok
            var conflictingReservation = await _context.Reservations
                .Where(r => r.RoomId == request.RoomId && 
                            r.Status != ReservationStatus.Rejected && 
                            r.StartTime < endUtc && 
                            r.EndTime > startUtc)
                .FirstOrDefaultAsync();

            if (conflictingReservation != null)
            {
                return BadRequest($"Ruangan {room.Name} sudah dipesan pada jam tersebut oleh {conflictingReservation.BorrowerName}.");
            }

            var reservation = new Reservation
            {
                BorrowerName = request.BorrowerName,
                RoomId = request.RoomId,
                StartTime = startUtc,
                EndTime = endUtc,
                Purpose = request.Purpose,
                Status = ReservationStatus.Pending
            };

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetReservations), new { id = reservation.Id }, reservation);
        }

        // PUT: api/reservations/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutReservation(int id, CreateReservationDto request)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null) return NotFound();

            if (reservation.Status != ReservationStatus.Pending)
            {
                return BadRequest("Hanya peminjaman berstatus Pending yang dapat diubah.");
            }

            var startUtc = request.StartTime.ToUniversalTime();
            var endUtc = request.EndTime.ToUniversalTime();

            var conflictingReservation = await _context.Reservations
                .Where(r => r.RoomId == request.RoomId && 
                            r.Id != id && 
                            r.Status != ReservationStatus.Rejected && 
                            r.StartTime < endUtc && 
                            r.EndTime > startUtc)
                .FirstOrDefaultAsync();

            if (conflictingReservation != null)
            {
                return BadRequest($"Bentrok! Ruangan sudah dipesan oleh {conflictingReservation.BorrowerName}.");
            }

            reservation.BorrowerName = request.BorrowerName;
            reservation.RoomId = request.RoomId;
            reservation.StartTime = startUtc;
            reservation.EndTime = endUtc;
            reservation.Purpose = request.Purpose;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // DELETE: api/reservations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteReservation(int id)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null) return NotFound();

            // Ambil role dari User yang sedang login (lewat Token JWT)
            var userRole = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;

            // Jika BUKAN Admin DAN statusnya BUKAN Pending, maka tolak.
            // Artinya: Admin bebas hapus (bypass). User cuma boleh hapus kalau Pending.
            if (userRole != "Admin" && reservation.Status != ReservationStatus.Pending)
            {
                return BadRequest("Peminjaman yang sudah diproses tidak dapat dihapus. Hubungi Admin.");
            }

            _context.Reservations.Remove(reservation);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // PUT: api/reservations/5/status
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/status")] 
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusDto request)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null) return NotFound(new { message = "Peminjaman tidak ditemukan." });

            if (request.Status == ReservationStatus.Approved)
            {
                var conflictingReservation = await _context.Reservations
                    .Where(r => r.RoomId == reservation.RoomId && 
                                r.Id != id && 
                                r.Status == ReservationStatus.Approved && 
                                r.StartTime < reservation.EndTime && 
                                r.EndTime > reservation.StartTime)
                    .FirstOrDefaultAsync();

                if (conflictingReservation != null)
                {
                    return BadRequest(new { message = $"Gagal Approve! Bentrok dengan {conflictingReservation.BorrowerName}." });
                }
            }

            reservation.Status = request.Status;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Status berhasil diperbarui", data = reservation });
        }
    }
}