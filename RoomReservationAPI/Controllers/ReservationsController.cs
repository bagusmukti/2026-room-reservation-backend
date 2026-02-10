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
                .ToListAsync();
        }

        // POST: api/reservations (Mengajukan Peminjaman)
        [HttpPost]
        public async Task<ActionResult<Reservation>> PostReservation(CreateReservationDto request)
        {
            // 1. Cek apakah ruangan ada?
            var room = await _context.Rooms.FindAsync(request.RoomId);
            if (room == null) return NotFound("Ruangan tidak ditemukan.");

            // 2. Validasi tanggal (Tidak boleh masa lalu)
            if (request.StartTime < DateTime.Now) 
                return BadRequest("Waktu mulai.");

            if (request.EndTime <= request.StartTime)
                return BadRequest("Waktu selesai.");

            // 3. Simpan data
            var reservation = new Reservation
            {
                BorrowerName = request.BorrowerName,
                RoomId = request.RoomId,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                Purpose = request.Purpose,
                Status = ReservationStatus.Pending // Default Pending
            };

            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetReservations), new { id = reservation.Id }, reservation);
        }

        // PATCH: api/reservations/{id}/status (Update Status: Approve/Reject)
        [HttpPatch("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, UpdateStatusDto request)
        {
            var reservation = await _context.Reservations.FindAsync(id);
            if (reservation == null) return NotFound();

            reservation.Status = request.Status;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}