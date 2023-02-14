using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace Hotel_Management_MVC.Models
{
    [Keyless]
    public class HotelBookings
    {
        public int HotelId { get; set; }

        public string HotelName { get; set; }

        public string GuestName { get; set; }

        [Display(Name = "No. of Guests")]
        public int NoOfGuests { get; set; }

        public string RoomType { get; set; }


        [Display(Name = "Booking Date")]
        public DateTime BookingDate { get; set; }

        [Display(Name = "Checkin Date")]
        public DateTime CheckInDate { get; set; }

        [Display(Name = "Checkout Date")]
        public DateTime CheckOutDate { get; set; }

        [Display(Name = "Id Proof")]
        public string IdProofType { get; set; }

        [Display(Name = "Id-Proof-No.")]
        public string IdProofNumber { get; set; }

        public string UserEmail { get; set; }

        [Display(Name = "No. of Rooms")]
        public int NoOfRooms { get; set; }

        [Display(Name = "Room Price")]
        public double RoomPrice { get; set; }

        [Display(Name = "Per Night")]
        public double PerNightRoomCharge { get; set; }

        [Display(Name = "Total Bill")]
        public double TotalBill { get; set; }


    }
}
