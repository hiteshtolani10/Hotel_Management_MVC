namespace Hotel_Management_MVC.Models
{
    public class UserEmailOptions
    {
        public List<string> ToEmails { get; set; }

        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
