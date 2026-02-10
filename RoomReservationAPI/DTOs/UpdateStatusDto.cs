using System.ComponentModel.DataAnnotations;
using RoomReservationAPI.Models;

namespace RoomReservationAPI.DTOs
{
    public class UpdateStatusDto
    {
        [Required]
        public ReservationStatus Status { get; set; }
    }
}