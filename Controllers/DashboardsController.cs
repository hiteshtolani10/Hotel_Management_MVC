using Hotel_Management_MVC.Models;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using System.Dynamic;

namespace Hotel_Management_MVC.Controllers
{
    public class DashboardsController : Controller
    {
        private readonly AdvancedTestDbContext _dbContext;
        public DashboardsController(AdvancedTestDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult UsersDashboard()
        {
            if (!String.IsNullOrEmpty(HttpContext.Session.GetString("UserEmail")))
            {
                List<SqlParameter> parameters = new List<SqlParameter>();
                Base _base = new Base();

                DataTable dt = _base.GetDataTable("sp_displayHotels", parameters, CommandType.StoredProcedure);

                List<HotelsAvailable> hotelList = new List<HotelsAvailable>();

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        HotelsAvailable hotel = new HotelsAvailable();

                        hotel.HotelId = Convert.ToInt32(row["HotelId"]);
                        hotel.HotelName = row["HotelName"].ToString();
                        hotel.DoubleBedAc = Convert.ToInt32(row["DoubleBedAc"]);
                        hotel.SingleBedAc = Convert.ToInt32(row["SingleBedAc"]);
                        hotel.DoubleBedNonAc = Convert.ToInt32(row["DoubleBedNonAc"]);
                        hotel.SingleBedNonAc = Convert.ToInt32(row["SingleBedNonAc"]);
                        hotel.MaharajaSuite = Convert.ToInt32(row["MaharajaSuite"]);
                        hotel.CoverImgUrl = row["CoverImgUrl"].ToString();
                        hotel.SingleBedAcImgUrl = row["SingleBedAcImgUrl"].ToString();
                        hotel.DoubleBedAcImgUrl = row["DoubleBedAcImgUrl"].ToString();
                        hotel.SingleBedNonAcImgUrl = row["SingleBedNonAcImgUrl"].ToString();
                        hotel.DoubleBedNonAcImgUrl = row["DoubleBedNonAcImgUrl"].ToString();
                        hotel.SingleBedAcPrice = (double)row["SingleBedAcPrice"];
                        hotel.DoubleBedAcPrice = (double)row["DoubleBedAcPrice"];
                        hotel.SingleBedNonAcPrice = (double)row["SingleBedNonAcPrice"];
                        hotel.DoubleBedNonAcPrice = (double)row["DoubleBedNonAcPrice"];


                        hotelList.Add(hotel);
                    }
                }
                return View(hotelList);

            }

            else
            {
                return RedirectToAction("UserLogin", "Accounts");
            }
        }

        public IActionResult HotelDashboard(string hotelName)
        {
            if (!String.IsNullOrEmpty(HttpContext.Session.GetString("HotelEmail")))
            {
                List<HotelBookings> bookingDetails = _dbContext.HotelBookings.Where(
                e => e.HotelName == hotelName).ToList();
                return View(bookingDetails);
            }

            else
            {
                return RedirectToAction("HotelLogin", "Accounts");
            }

        }

        public IActionResult AdminDashboard()
        {
            if (!String.IsNullOrEmpty(HttpContext.Session.GetString("AdminEmail")))
            {
                List<HotelDetail> hotelDetails = _dbContext.HotelDetails.ToList();
                List<UserDetails> userDetails = _dbContext.UserDetails.ToList();

                dynamic adminDashboardDetails = new ExpandoObject();

                adminDashboardDetails.HotelDetail = hotelDetails;
                adminDashboardDetails.UserDetails = userDetails;

                return View(adminDashboardDetails);
            }   

            else
            {
                return RedirectToAction("AdminLogin", "Accounts");
            }
        }

        public IActionResult GrantHotelAccess(string id)
        {

            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@id", id));
            Base _base = new Base(); 

            _base.GetDataTable("sp_updateHtelDetails", parameters, CommandType.StoredProcedure);
            SendAcceptanceEmail(id);
            return RedirectToAction("AdminDashboard");

        }

        public void SendAcceptanceEmail(string id)
        {
            var hotel = _dbContext.HotelDetails.SingleOrDefault(e => e.Id == id);
            if(hotel != null)
            {
                try
                {
                    string emailId = hotel.Email;
                    var verifyUrl = "/Accounts/HotelLogin";
                    string url = Request.GetDisplayUrl();
                    var link = url.Replace("/Dashboards/GrantHotelAccess/" + id, verifyUrl);

                    var fromEmail = new MailAddress("tolanihitesh1018@gmail.com", "Hotel Management");
                    var toEmail = new MailAddress(emailId);
                    var fromEmailPassword = "lzubezgjkphblnfm";
                    string subject = "Application Accepted!";

                    string body = "<br/><h3>Greetings!<h3><br/>" +
                        "Welcome to Infinite Hotels<br/>" +
                        "<br/>We would like to inform you that your Application has been accepted<br/>" +
                        "<br/>and your Hotel is successfully Registered!<br/>" +
                        "<br/>Please follow the link below to Login to your Account!<br/>" +
                        "<br/> <a href = '" + link + "'> Login </a>";

                    var smtpRequest = new SmtpClient
                    {
                        Host = "smtp.gmail.com",
                        Port = 587,
                        EnableSsl = true,
                        DeliveryMethod = SmtpDeliveryMethod.Network,
                        UseDefaultCredentials = false,
                        Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
                    };

                    using (var message = new MailMessage(fromEmail, toEmail)
                    {
                        Subject = subject,
                        Body = body,
                        IsBodyHtml = true
                    }) smtpRequest.Send(message);

                }

                catch (Exception ex)
                {
                    throw ex;
                };
            }
            
        }

        public async Task<IActionResult> SelectRooms(int id)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@id", id));
            Base _base = new Base();

            DataTable dt = _base.GetDataTable("sp_displayHotels", parameters, CommandType.StoredProcedure);

            List<HotelsAvailable> hotelList = new List<HotelsAvailable>();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    HotelsAvailable hotel = new HotelsAvailable();

                    hotel.HotelId = Convert.ToInt32(row["HotelId"]);
                    hotel.HotelName = row["HotelName"].ToString();
                    hotel.DoubleBedAc = Convert.ToInt32(row["DoubleBedAc"]);
                    hotel.SingleBedAc = Convert.ToInt32(row["SingleBedAc"]);
                    hotel.DoubleBedNonAc = Convert.ToInt32(row["DoubleBedNonAc"]);
                    hotel.SingleBedNonAc = Convert.ToInt32(row["SingleBedNonAc"]);
                    hotel.MaharajaSuite = Convert.ToInt32(row["MaharajaSuite"]);
                    hotel.CoverImgUrl = row["CoverImgUrl"].ToString();
                    hotel.SingleBedAcImgUrl = row["SingleBedAcImgUrl"].ToString();
                    hotel.DoubleBedAcImgUrl = row["DoubleBedAcImgUrl"].ToString();
                    hotel.SingleBedNonAcImgUrl = row["SingleBedNonAcImgUrl"].ToString();
                    hotel.DoubleBedNonAcImgUrl = row["DoubleBedNonAcImgUrl"].ToString();
                    hotel.SingleBedAcPrice = (double)row["SingleBedAcPrice"];
                    hotel.DoubleBedAcPrice = (double)row["DoubleBedAcPrice"];
                    hotel.SingleBedNonAcPrice = (double)row["SingleBedNonAcPrice"];
                    hotel.DoubleBedNonAcPrice = (double)row["DoubleBedNonAcPrice"];

                    hotelList.Add(hotel);
                }
            }
            return View(hotelList);
        }

        public async Task<IActionResult> BookRoom(string roomType, int id)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@id", id));
            Base _base = new Base();

            DataTable dt = _base.GetDataTable("sp_displayHotels", parameters, CommandType.StoredProcedure);

            BookingDetails bookingDetails = new BookingDetails();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    bookingDetails.HotelId = Convert.ToInt32(row["HotelId"]);
                    bookingDetails.HotelName = row["HotelName"].ToString();
                    bookingDetails.HotelCoverImg = row["CoverImgUrl"].ToString();
                    bookingDetails.UserEmail = HttpContext.Session.GetString("UserEmail");
                    bookingDetails.NoOfRooms = 1;

                    if (roomType == "DoubleBedAc")
                    {
                        bookingDetails.RoomTypeNo = Convert.ToInt32(row["DoubleBedAc"]);
                        bookingDetails.NoOfGuests = 2;
                        bookingDetails.RoomTypeString = "Double Bed Ac";
                        bookingDetails.RoomTypeImg = row["DoubleBedAcImgUrl"].ToString();
                        bookingDetails.RoomPrice = (double)row["DoubleBedAcPrice"];
                        bookingDetails.AmountPerNight = (double)row["DoubleBedAcPrice"];
                        bookingDetails.maxGuests = 3;
                    }

                    else if (roomType == "SingleBedAc")
                    {
                        bookingDetails.RoomTypeNo = Convert.ToInt32(row["SingleBedAc"]);
                        bookingDetails.NoOfGuests = 1;
                        bookingDetails.RoomTypeString = "Single Bed Ac";
                        bookingDetails.RoomTypeImg = row["SingleBedAcImgUrl"].ToString();
                        bookingDetails.RoomPrice = (double)row["SingleBedAcPrice"];
                        bookingDetails.AmountPerNight = (double)row["SingleBedAcPrice"];
                        bookingDetails.maxGuests = 2;
                    }

                    else if (roomType == "DoubleBedNonAc")
                    {
                        bookingDetails.RoomTypeNo = Convert.ToInt32(row["DoubleBedNonAc"]);
                        bookingDetails.NoOfGuests = 2;
                        bookingDetails.RoomTypeString = "Double Bed Non Ac";
                        bookingDetails.RoomTypeImg = row["DoubleBedNonAcImgUrl"].ToString();
                        bookingDetails.RoomPrice = (double)row["DoubleBedNonAcPrice"];
                        bookingDetails.AmountPerNight = (double)row["DoubleBedNonAcPrice"];
                        bookingDetails.maxGuests = 3;
                    }

                    else if (roomType == "SingleBedNonAc")
                    {
                        bookingDetails.RoomTypeNo = Convert.ToInt32(row["SingleBedNonAc"]);
                        bookingDetails.NoOfGuests = 1;
                        bookingDetails.RoomTypeString = "Single Bed Non Ac";
                        bookingDetails.RoomTypeImg = row["SingleBedNonAcImgUrl"].ToString();
                        bookingDetails.RoomPrice = (double)row["SingleBedNonAcPrice"];
                        bookingDetails.AmountPerNight = (double)row["SingleBedNonAcPrice"];
                        bookingDetails.maxGuests = 2;
                    }

                }
            }
            return View(bookingDetails);
        }

        [HttpPost]
        public async Task<IActionResult> BookRoom(BookingDetails details)
        {
            if(ModelState.IsValid)
            {
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@hotelId", details.HotelId));
                parameters.Add(new SqlParameter("@hotelName", details.HotelName));
                parameters.Add(new SqlParameter("@guestName", details.Name));
                parameters.Add(new SqlParameter("@noOfGuests", details.NoOfGuests));
                parameters.Add(new SqlParameter("@roomType", details.RoomTypeString));
                parameters.Add(new SqlParameter("@checkInDate", details.CheckInDate));
                parameters.Add(new SqlParameter("@checkOutDate", details.CheckOutDate));
                parameters.Add(new SqlParameter("@idProofType", details.IdProofType));
                parameters.Add(new SqlParameter("@idProofNumber", details.IdProofNumber));
                parameters.Add(new SqlParameter("@noOfRooms", details.NoOfRooms));
                parameters.Add(new SqlParameter("@roomPrice", details.RoomPrice));
                parameters.Add(new SqlParameter("@perNightRoomCharge", details.AmountPerNight));
                parameters.Add(new SqlParameter("@totalBill", details.TotalAmount));
                parameters.Add(new SqlParameter("@userEmail", details.UserEmail));
                
                Base _base = new Base();

                DataTable dt = _base.GetDataTable("sp_BookRoomForUser", parameters, CommandType.StoredProcedure);

                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        if (row["Inserted"].ToString() == "201")
                        {
                            return RedirectToAction("BookingSummary", details);
                        }
                        else
                        {
                            ViewBag.SomethingWentWrong = "Something went Wrong";
                            return View(details);
                        }
                    }
                }
            }
            return View(details);
        }

        public IActionResult BookingSummary(BookingDetails details)
        {
            return View(details);
        }
        
        public IActionResult UserBookingsTab(string email)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            parameters.Add(new SqlParameter("@userEmail", email));
            Base _base = new Base();

            DataTable dt = _base.GetDataTable("sp_GetUserBookings", parameters, CommandType.StoredProcedure);

            List<BookingDetails> bookingsList = new List<BookingDetails>();
            if (dt != null && dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    BookingDetails booking = new BookingDetails();

                    booking.Name = row["GuestName"].ToString();
                    booking.HotelName = row["HotelName"].ToString();
                    booking.NoOfGuests = Convert.ToInt32(row["NoOfGuests"]);
                    booking.NoOfRooms = Convert.ToInt32(row["NoOfRooms"]);
                    booking.RoomTypeString = row["RoomType"].ToString();
                    booking.CheckInDate = (DateTime) row["CheckInDate"];
                    booking.CheckOutDate = (DateTime) row["CheckOutDate"];
                    booking.RoomPrice = Convert.ToInt32(row["RoomPrice"]);
                    booking.AmountPerNight = Convert.ToInt32(row["PerNightRoomCharge"]);
                    booking.TotalAmount = Convert.ToInt32(row["TotalBill"]);
                    booking.IdProofType = row["IdProofType"].ToString();

                    bookingsList.Add(booking);
                }
                return View(bookingsList);
            }
            else
            {
                return View(bookingsList);
            }
            
        }

        public IActionResult DeleteHotel(int id)
        {
            return View();
        }

    }
}
