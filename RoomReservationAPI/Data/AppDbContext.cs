using Microsoft.EntityFrameworkCore;
using RoomReservationAPI.Models;

namespace RoomReservationAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        // Daftar Model agar jadi tabel di database
        public DbSet<Room> Rooms { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

        // Seeder
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Room>().HasData(
                new Room { Id = 1, Name = "C 302", Location = "Gedung D4 Lt. 3", Capacity = 30, Description = "Laboratorium Jaringan Komputer" },
                new Room { Id = 2, Name = "06.10", Location = "Gedung SAW Lt. 6", Capacity = 60, Description = "Ruang Kelas Gabungan" },
                new Room { Id = 3, Name = "HH.212", Location = "Gedung D3 Lt. 2", Capacity = 30, Description = "Ruang Kelas" }
            );
        }
    }
}