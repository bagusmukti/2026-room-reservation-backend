using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using RoomReservationAPI.Models;

namespace RoomReservationAPI.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Room> Rooms { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(warnings => 
                warnings.Ignore(RelationalEventId.PendingModelChangesWarning));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Seeding Role
            var adminRoleId = "role-admin-id";
            var userRoleId = "role-user-id";

            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole { Id = adminRoleId, Name = "Admin", NormalizedName = "ADMIN" },
                new IdentityRole { Id = userRoleId, Name = "User", NormalizedName = "USER" }
            );

            // Seeding Admin
            var adminUserId = "user-admin-id";
            var adminUser = new IdentityUser
            {
                Id = adminUserId,
                UserName = "admin",
                NormalizedUserName = "ADMIN",
                Email = "admin@kampus.ac.id",
                NormalizedEmail = "ADMIN@KAMPUS.AC.ID",
                EmailConfirmed = true,
                // KUNCI NILAI INI AGAR TIDAK ACAK:
                SecurityStamp = "static-security-stamp", 
                ConcurrencyStamp = "static-concurrency-stamp"
            };

            // Hash Password "Admin#123"
            PasswordHasher<IdentityUser> ph = new PasswordHasher<IdentityUser>();
            adminUser.PasswordHash = ph.HashPassword(adminUser, "Admin#123");

            modelBuilder.Entity<IdentityUser>().HasData(adminUser);

            // Menggabungkan User dengan Role
            modelBuilder.Entity<IdentityUserRole<string>>().HasData(
                new IdentityUserRole<string>
                {
                    RoleId = adminRoleId,
                    UserId = adminUserId
                }
            );

            // Seeding Room
            modelBuilder.Entity<Room>().HasData(
                new Room { Id = 1, Name = "C 302", Location = "Gedung D4 Lt. 3", Capacity = 30, Description = "Laboratorium Jaringan Komputer" },
                new Room { Id = 2, Name = "06.10", Location = "Gedung SAW Lt. 6", Capacity = 60, Description = "Ruang Kelas Gabungan" },
                new Room { Id = 3, Name = "HH.212", Location = "Gedung D3 Lt. 2", Capacity = 30, Description = "Ruang Kelas" }
            );
        }
    }
}