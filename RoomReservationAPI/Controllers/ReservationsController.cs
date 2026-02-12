using Microsoft.AspNetCore.Authorization; // Tambahan wajib untuk [Authorize]
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RoomReservationAPI.Data;
using RoomReservationAPI.Models;
using RoomReservationAPI.DTOs;

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

        // GET: api/reservations (Melihat semua peminjaman + Data Ruangannya)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Reservation>>> GetReservations()
        {
            return await _context.Reservations
                .Include(r => r.Room) // Join table Room biar nama ruangan muncul
                .OrderByDescending(r => r.StartTime) // Biar yang terbaru muncul di atas
                .ToListAsync();
        }

        // POST: api/reservations (Mengajukan Peminjaman)
        [HttpPost]
        public async Task<ActionResult<Reservation>> PostReservation(CreateReservationDto request)
        {
            // 1. Cek apakah ruangan ada?
            var room = await _context.Rooms.FindAsync(request.RoomId);
            if (room == null) return NotFound("Ruangan tidak ditemukan.");

            // 2. Validasi tanggal
            if (request.StartTime < DateTime.Now) 
                return BadRequest("Waktu mulai tidak boleh di masa lalu.");

            if (request.EndTime <= request.StartTime)
                return BadRequest("Waktu selesai harus lebih besar dari waktu mulai.");

            // 3. Simpan data
            var reservation = new Reservation
            {
                BorrowerName = request.BorrowerName,
                RoomId = request.RoomId,
                StartTime = request.StartTime.ToUniversalTime(),
                EndTime = request.EndTime.ToUniversalTime(),
                Purpose = request.Purpose,
                Status = ReservationStatus.Pending
            };

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetReservations), new { id = reservation.Id }, reservation);
        }

        // PUT: api/reservations/{id}/status (Update Status: Approve/Reject)
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/status")] 
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] UpdateStatusDto request)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null) return NotFound(new { message = "Peminjaman tidak ditemukan." });

            // Update status
            reservation.Status = request.Status;
            await _context.SaveChangesAsync();

            // Return OK dengan pesan JSON 
            return Ok(new { message = "Status berhasil diperbarui", data = reservation });
        }
    }
}