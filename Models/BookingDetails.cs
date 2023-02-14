using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Hotel_Management_MVC.Models
{
    [Keyless]
    public class BookingDetails
    {
        public int HotelId { get; set; }

        public string HotelName { get; set; }

        public string HotelCoverImg { get; set; }

        public int RoomTypeNo { get; set; }

        public string RoomTypeString { get; set; }

        public string RoomTypeImg { get; set; }

        [Required, Display(Name = "Full Name")]
        public string Name { get; set; }

        [Display(Name = "Check In")]
        public DateTime CheckInDate { get; set; } = DateTime.Now.AddDays(1);

        [Display(Name = "Check Out")]
        public DateTime CheckOutDate { get; set; } = DateTime.Now.AddDays(2);

        [Display(Name = "No. of Guests")]
        public int NoOfGuests { get; set;}

        [Display(Name = "Room Price Rs.")]
        public double RoomPrice { get; set; }

        [Display(Name = "Per Night Rs.")]
        public double AmountPerNight { get; set; }

        [Display(Name = "Total Amount Rs.")]
        public double TotalAmount { get; set; }

        [Required]
        [Display(Name = "Id Proof")]
        public string IdProofType { get; set; }

        [Required]
        [Display(Name = "Id-Proof-No.")]
        public string IdProofNumber { get; set;}

        public string UserEmail { get; set;}

        [Display(Name = "No. of Rooms")]
        public int NoOfRooms { get; set; }

        public int? maxGuests { get; set; }

    }
}
