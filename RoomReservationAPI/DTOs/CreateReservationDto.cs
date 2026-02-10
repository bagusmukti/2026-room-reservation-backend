using System.ComponentModel.DataAnnotations;

namespace RoomReservationAPI.DTOs
{
    public class CreateReservationDto
    {
        [Required]
        public string BorrowerName { get; set; } = string.Empty;

        [Required]
        public int RoomId { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Required]
        public string Purpose { get; set; } = string.Empty;
    }
}