using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RoomReservationAPI.Models
{
    public enum ReservationStatus
    {
        Pending,
        Approved,
        Rejected
    }

    public class Reservation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string BorrowerName { get; set; } = string.Empty; // Borrower's Name

        [Required]
        public DateTime StartTime { get; set; } // Start Time

        [Required]
        public DateTime EndTime { get; set; } // End Time

        public string Purpose { get; set; } = string.Empty; // Purpose

        public ReservationStatus Status { get; set; } = ReservationStatus.Pending; // Default: Pending

        // Relation to Room (Foreign Key)
        [Required]
        public int RoomId { get; set; }

        [ForeignKey("RoomId")]
        public Room? Room { get; set; }
    }
}