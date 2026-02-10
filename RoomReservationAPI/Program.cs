using Microsoft.EntityFrameworkCore;
using RoomReservationAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// --- 1. DAFTARKAN SERVICE ---

// PENTING: Aktifkan fitur Controller
builder.Services.AddControllers();

// Konfigurasi OpenAPI/Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); 

// Konfigurasi Database PostgreSQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// --- 2. KONFIGURASI PIPELINE ---

// Aktifkan Swagger UI (untuk testing visual)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// PENTING: Petakan Controller agar bisa diakses
app.MapControllers();

app.Run();