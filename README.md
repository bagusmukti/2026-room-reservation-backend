# Sistem Peminjaman Ruangan (Backend)

Backend API untuk sistem peminjaman ruangan kampus, dibangun menggunakan ASP.NET Core.

## 🚀 Fitur
- **Autentikasi & Otorisasi:** Login, Register, dan Role-based Access (Admin & User).
- **Manajemen Ruangan (CRUD):** Tambah, lihat, ubah, dan hapus data ruangan (Admin).
- **Peminjaman Ruangan:** - User: Mengajukan, Mengedit, Membatalkan peminjaman.
  - Admin: Menyetujui (Approve), Menolak (Reject), dan Re-evaluasi status.
- **Validasi Cerdas:** Mencegah peminjaman ganda (double booking) pada waktu yang sama.
- **Arsitektur:** Menggunakan pola Controller, Service, dan Repository (via DTO).
- **Database:** PostgreSQL dengan Entity Framework Core.

## 🛠 Teknologi
- .NET 10 SDK
- ASP.NET Core Web API
- PostgreSQL
- Entity Framework Core

## 📦 Instalasi
1. Clone repositori:
   ```bash
   git clone [https://github.com/bagusmukti/2026-room-reservation-backend.git](https://github.com/bagusmukti/2026-room-reservation-backend.git)