using System.ComponentModel.DataAnnotations;

namespace RoomReservationAPI.DTOs
{
    public class CreateRoomDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Location { get; set; } = string.Empty;

        [Range(1, 1000)] // Validasi kapasitas minimal 1
        public int Capacity { get; set; }

        public string Description { get; set; } = string.Empty;
    }
}