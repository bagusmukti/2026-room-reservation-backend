using System.ComponentModel.DataAnnotations;

namespace RoomReservationAPI.Models
{
    public class Room
    {
        [Key]
        public int Id { get; set; }

        [Required] 
        public string Name { get; set; } = string.Empty;

        [Required]
        public string Location { get; set; } = string.Empty;

        public int Capacity { get; set; }

        public string Description { get; set; } = string.Empty;
    }
}