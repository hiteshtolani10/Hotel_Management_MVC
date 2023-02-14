using System.ComponentModel.DataAnnotations;

namespace Hotel_Management_MVC.Models
{
    public class HotelsAvailable
    {
        public int? HotelId { get; set; }
        public string HotelName { get; set; }
        
        [Display(Name = "Single-Bed")]
        public int SingleBedAc { get; set; }

        [Display(Name = "Single-Bed")]
        public int SingleBedNonAc { get; set; }

        [Display(Name = "Double-Bed")]
        public int DoubleBedAc { get; set; }

        [Display(Name = "Double-Bed")]
        public int DoubleBedNonAc { get; set; }

        public int MaharajaSuite { get; set; }

        public string CoverImgUrl { get; set; }

        public string? SingleBedAcImgUrl { get; set; }

        public string? DoubleBedAcImgUrl { get; set; }

        public string? SingleBedNonAcImgUrl { get; set; }

        public string? DoubleBedNonAcImgUrl { get; set; }

        public double? SingleBedAcPrice { get; set; }

        public double? DoubleBedAcPrice { get; set; }

        public double? SingleBedNonAcPrice { get; set; }

        public double? DoubleBedNonAcPrice { get; set; }

    }
}
