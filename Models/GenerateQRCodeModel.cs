using System.ComponentModel.DataAnnotations;

namespace Hotel_Management_MVC.Models
{
    public class GenerateQRCodeModel
    {
        [Display(Name = "Enter QR Code Text")]
        public string QRCodeText
        {
            get;
            set;
        }
    }
}
