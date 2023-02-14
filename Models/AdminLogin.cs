using System.ComponentModel.DataAnnotations;

namespace Hotel_Management_MVC.Models
{
    public class AdminLogin
    {
        [Required(ErrorMessage = "Please Enter Admin Email")]
        [Display(Name = "Admin Email")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please Enter Your Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

    }
}
