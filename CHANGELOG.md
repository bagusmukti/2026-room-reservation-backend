# Changelog

Semua perubahan penting pada proyek ini akan didokumentasikan dalam file ini.

Format mengikuti [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
dan proyek ini mematuhi [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2026-02-10

### Added
- Fitur CRUD lengkap untuk Master Ruangan (Create, Read, Update, Delete).
- Implementasi DTO (Data Transfer Object) untuk request dan response.
- Validasi input pada controller ruangan.
- Konfigurasi database PostgreSQL dan Entity Framework Core.
- Setup Swagger UI untuk dokumentasi API.

### Changed
- Refactor `RoomsController` untuk menggunakan DTO pattern.