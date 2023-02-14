using Hotel_Management_MVC.Models;

namespace Hotel_Management_MVC.Services
{
    public interface IEmailServices
    {
        Task SendTestEmail(UserEmailOptions emailOptions);
    }
}