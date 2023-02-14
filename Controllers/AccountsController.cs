using Hotel_Management_MVC.Models;
using Hotel_Management_MVC.Services;
using IronBarCode;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Emit;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using System.Data;
using System.Drawing;
using System.Net;
using System.Net.Mail;
using System.Text;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Hotel_Management_MVC.Controllers
{
    public class AccountsController : Controller
    {

        string registrationApiUrl = "https://localhost:7047/api/HotelsApi/";
        private readonly AdvancedTestDbContext _dbContext;
        private IWebHostEnvironment Environment;

        //temp-delete 
        private readonly IEmailServices _emailServices;

        public AccountsController(AdvancedTestDbContext dbContext, IEmailServices emailServices,
            IWebHostEnvironment _environment)
        {
            _dbContext = dbContext;
            _emailServices = emailServices;
            Environment = _environment;
        }

        public IActionResult Index()
        {
            return View();
        }

        //User Section
        [HttpGet]
        public IActionResult UserLogin()
        {
            return View();
        }

        [HttpPost]
        public IActionResult UserLogin(UserLogin model)
        {
            if (ModelState.IsValid)
            {
                var user = _dbContext.UserDetails.SingleOrDefault(e => e.Email == model.Email);
                if (user != null)
                {
                    string decryptedPassword = Base.DecryptPassword(user.Password);
                    if (decryptedPassword == model.Password)
                    {
                        if (user.EmailVerified == false)
                        {
                            ViewBag.UserLoginError = "Email Verification is Pending!";
                            return View(model);
                        }
                        else
                        {
                            HttpContext.Session.SetString("UserEmail", model.Email);
                            return RedirectToAction("UsersDashboard", "Dashboards");
                        }

                    }
                    else
                    {
                        ViewBag.UserLoginError = "Incorrect Password";
                        return View(model);
                    }
                }
                else
                {
                    ViewBag.UserLoginError = "No user found with this Username";
                    return View(model);
                }
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult UserRegister()
        {
            return View();
        }

        [HttpPost]
        public IActionResult UserRegister(UserRegister model)
        {
            if(ModelState.IsValid)
            {

                string activationCode = Guid.NewGuid().ToString();
                List<SqlParameter> parameters = new List<SqlParameter>();
                string encryptedPassword = Base.EncryptPassword(model.Password);
                parameters.Add(new SqlParameter("@id", Guid.NewGuid().ToString()));
                parameters.Add(new SqlParameter("@firstName", model.FirstName));
                parameters.Add(new SqlParameter("@lastName", model.LastName));
                parameters.Add(new SqlParameter("@email", model.Email));
                parameters.Add(new SqlParameter("@password", encryptedPassword));
                parameters.Add(new SqlParameter("@creationDate", DateTime.Now));
                parameters.Add(new SqlParameter("@activationCode", activationCode));

                Base _base = new Base();

                DataTable dt = _base.GetDataTable("sp_addNewUser", parameters, CommandType.StoredProcedure);
                if(dt != null && dt.Rows.Count > 0)
                {
                    foreach(DataRow row in dt.Rows)
                    {
                        if (row["Inserted"].ToString() == "1")
                        {
                            
                            SendVerificationLinkEmail(model.Email, activationCode);
                            return View("WaitingEmailConfirmation");
                        }
                        else
                        {
                            ViewBag.AlreadyExist = "User with this Email Already Exist!";
                            return View(model);
                        }
                    }
                }

            }
            return View(model);
        }

        [HttpGet]
        public IActionResult UserLogout()
        {
            HttpContext.Session.Remove("UserEmail");
            HttpContext.Session.Clear();
            return RedirectToAction("UserLogin");
        }

        //Hotel Section
        [HttpGet]
        public IActionResult HotelLogin()
        {
            return View();
        }

        [HttpPost]
        public IActionResult HotelLogin(HotelDetail model)
        {
            var hotel = _dbContext.HotelDetails.SingleOrDefault(e=>e.Email == model.Email);

            if(hotel != null)
            {
                if(hotel.EmailVerified == false)
                {
                    //Change to Grant Access or Email verified
                    ViewBag.HotelLoginError = "Hotel Email is not Verified Yet";
                    return View(model);
                }
                else
                {
                    string decryptedPassword = Base.DecryptPassword(hotel.Password);
                    if(model.Password == decryptedPassword)
                    {
                        string hotelName = hotel.HotelName;
                        HttpContext.Session.SetString("HotelEmail", model.Email);
                        return RedirectToAction("HotelDashboard", "Dashboards", new { hotelName });
                    }
                    else
                    {
                        ViewBag.HotelLoginError = "Wrong Password!";
                        return View(model);
                    }
                    
                }
            }
            return View(model);

        }

        [HttpGet]
        public IActionResult HotelRegister()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> HotelRegister(HotelRegister hotel)
        {
            if (ModelState.IsValid)
            {
                if(hotel.CoverImage != null)
                {
                    string imgUrl = "HotelImages/CoverPhotos/";
                    string coverImgName = hotel.HotelName.ToString() + "_CoverImage_";
                    //hotel.CoverImage.Name.Replace(hotel.CoverImage.Name,coverImgName);
                    imgUrl += coverImgName + hotel.CoverImage.FileName;
                    string root = Path.Combine(Environment.WebRootPath, imgUrl);
                    hotel.CoverImageUrl = "/" + imgUrl;
                    await hotel.CoverImage.CopyToAsync(new FileStream(root, FileMode.Create));
                }

                if (hotel.SingleBedAcImg != null)
                {
                    string SingleBedAcImgUrl = "HotelImages/Rooms/";
                    string SingleBedAcImgName = hotel.HotelName.ToString() + "_SingleBedAcImg_";
                    //hotel.CoverImage.Name.Replace(hotel.CoverImage.Name,coverImgName);
                    SingleBedAcImgUrl += SingleBedAcImgName + hotel.SingleBedAcImg.FileName;
                    string root = Path.Combine(Environment.WebRootPath, SingleBedAcImgUrl);
                    hotel.SingleBedAcImgUrl = "/" + SingleBedAcImgUrl;
                    await hotel.SingleBedAcImg.CopyToAsync(new FileStream(root, FileMode.Create));
                }

                if (hotel.DoubleBedAcImg != null)
                {
                    string DoubleBedAcImgUrl = "HotelImages/Rooms/";
                    string DoubleBedAcImgName = hotel.HotelName.ToString() + "_DoubleBedAcImg_";
                    //hotel.CoverImage.Name.Replace(hotel.CoverImage.Name,coverImgName);
                    DoubleBedAcImgUrl += DoubleBedAcImgName + hotel.DoubleBedAcImg.FileName;
                    string root = Path.Combine(Environment.WebRootPath, DoubleBedAcImgUrl);
                    hotel.DoubleBedAcImgUrl = "/" + DoubleBedAcImgUrl;
                    await hotel.DoubleBedAcImg.CopyToAsync(new FileStream(root, FileMode.Create));
                }

                if (hotel.SingleBedNonAcImg != null)
                {
                    string SingleBedNonAcImgUrl = "HotelImages/Rooms/";
                    string SingleBedNonAcImgName = hotel.HotelName.ToString() + "_SingleBedNonAcImg_";
                    //hotel.CoverImage.Name.Replace(hotel.CoverImage.Name,coverImgName);
                    SingleBedNonAcImgUrl += SingleBedNonAcImgName + hotel.SingleBedNonAcImg.FileName;
                    string root = Path.Combine(Environment.WebRootPath, SingleBedNonAcImgUrl);
                    hotel.SingleBedNonAcImgUrl = "/" + SingleBedNonAcImgUrl;
                    await hotel.SingleBedNonAcImg.CopyToAsync(new FileStream(root, FileMode.Create));
                }

                if (hotel.DoubleBedNonAcImg != null)
                {
                    string DoubleBedNonAcImgUrl = "HotelImages/Rooms/";
                    string DoubleBedNonAcImgName = hotel.HotelName.ToString() + "_DoubleBedNonAcImg_";
                    //hotel.CoverImage.Name.Replace(hotel.CoverImage.Name,coverImgName);
                    DoubleBedNonAcImgUrl += DoubleBedNonAcImgName + hotel.DoubleBedNonAcImg.FileName;
                    string root = Path.Combine(Environment.WebRootPath, DoubleBedNonAcImgUrl);
                    hotel.DoubleBedNonAcImgUrl = "/" + DoubleBedNonAcImgUrl;
                    await hotel.DoubleBedNonAcImg.CopyToAsync(new FileStream(root, FileMode.Create));
                }

                HotelDetail details = new HotelDetail();

                details.Id = Guid.NewGuid().ToString();
                details.HotelName = hotel.HotelName;
                details.Email = hotel.Email;
                details.Password = Base.EncryptPassword(hotel.Password);
                details.EmailVerified = false;
                details.IsActive = true;
                details.IsDeleted = false;
                details.CreationDate = DateTime.Now;
                details.SingleBedAc = hotel.SingleBedAc;
                details.SingleBedNonAc = hotel.SingleBedNonAc;
                details.DoubleBedAc = hotel.DoubleBedAc;
                details.DoubleBedNonAc = hotel.DoubleBedNonAc;
                details.MaharajaSuite = hotel.MaharajaSuite;
                details.CoverImageUrl = hotel.CoverImageUrl;
                details.SingleBedAcPrice = (double)hotel.SingleBedAcPrice;
                details.DoubleBedAcPrice = (double)hotel.DoubleBedAcPrice;
                details.SingleBedNonAcPrice = (double)hotel.SingleBedNonAcPrice;
                details.DoubleBedNonAcPrice = (double)hotel.DoubleBedNonAcPrice;
                details.SingleBedAcImgUrl = hotel.SingleBedAcImgUrl;
                details.DoubleBedAcImgUrl = hotel.DoubleBedAcImgUrl;
                details.SingleBedNonAcImgUrl = hotel.SingleBedNonAcImgUrl;
                details.DoubleBedNonAcImgUrl = hotel.DoubleBedNonAcImgUrl;

                //_dbContext.HotelDetails.Add(details);
                //_dbContext.SaveChanges();
                //return View("WaitingRegistration");

                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri(registrationApiUrl);
                    StringContent content = new StringContent(JsonConvert.SerializeObject(details),
                        Encoding.UTF8, "application/json");
                    var response = await client.PostAsync(registrationApiUrl, content);
                    var status = response.StatusCode;

                    if (status == System.Net.HttpStatusCode.Created)
                    {
                        return View("WaitingRegistration");
                    }

                    else if (status == System.Net.HttpStatusCode.AlreadyReported)
                    {
                        ViewBag.AlreadyExist = "Hotel with this Email Already Exist!";
                        return View(hotel);
                    }
                }
            }

            return View(hotel);
        }

        [HttpGet]
        public IActionResult HotelLogout()
        {
            HttpContext.Session.Remove("HotelEmail");
            HttpContext.Session.Clear();
            return RedirectToAction("HotelLogin");
        }

        //Admin Section
        [HttpGet]
        public IActionResult AdminLogin()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AdminLogin(AdminLogin model)
        {
            if (ModelState.IsValid)
            {
                var admin = _dbContext.AdminDetails.SingleOrDefault(e => e.Email == model.Email);
                if (admin != null)
                {
                    string decryptedPassword = Base.DecryptPassword(admin.Password);
                    if(model.Password == decryptedPassword)
                    {
                        HttpContext.Session.SetString("AdminEmail", model.Email);
                        return RedirectToAction("AdminDashboard", "Dashboards");
                    }
                    else
                    {
                        ViewBag.AdminLoginError = "Incorrect Password";
                        return View(model);
                    }
                }
                else
                {
                    ViewBag.AdminLoginError = "No Admin with this Email Exists!";
                    return View(model);
                }
            }
            return View(model);

        }

        //email verification link
        public void SendVerificationLinkEmail(string emailId, string activationCode)
        {
            try
            {
                var verifyUrl = "/Accounts/VerifyAccount?code=" + activationCode;
                string url = Request.GetDisplayUrl();
                var link = url.Replace("/Accounts/UserRegister", verifyUrl);

                var fromEmail = new MailAddress("tolanihitesh1018@gmail.com", "Hotel Management");
                var toEmail = new MailAddress(emailId);
                var fromEmailPassword = "lzubezgjkphblnfm";
                string subject = "Account Verification";

                string body = "<br/><h3>Greetings!<h3><br/>" +
                    "<br/>Welcome to Infinite Hotels<br/>" +
                    "<br/>Verify your account to continue<br/>" +
                    "<br/> <a href = '" + link + "'> Verify </a>";

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

            catch(Exception ex)
            {
                throw ex;
            };    
        }

        public ActionResult VerifyAccount(string code)
        {
            if (!String.IsNullOrEmpty(code))
            {
                List<SqlParameter> parameters = new List<SqlParameter>();
                parameters.Add(new SqlParameter("@verificationCode", code));

                Base _base = new Base();

                DataTable dt = _base.GetDataTable("sp_verifyEmail", parameters, CommandType.StoredProcedure);
                if (dt != null && dt.Rows.Count > 0)
                {
                    foreach (DataRow row in dt.Rows)
                    {
                        if (row["Inserted"].ToString() == "1")
                        {
                            return View();
                        }
                        else
                        {
                            ViewBag.AlreadyExist = "Something Went Wrong!";
                            return View();
                        }
                    }
                }
                
            }
            ViewBag.AlreadyExist = "Something Went Wrong!";
            return View();

        }

        [HttpGet]
        public IActionResult AdminRegister()
        {
            return View();
        }

        [HttpPost]
        public IActionResult AdminRegister(AdminRegister admin)
        {
            if (ModelState.IsValid)
            {
                AdminDetails details = new AdminDetails();
                details.Id = Guid.NewGuid().ToString();
                details.FirstName = admin.FirstName;
                details.LastName = admin.LastName;
                details.Email = admin.Email;
                details.Password = admin.Password;
                details.EmailVerified = true;
                details.CreationDate = DateTime.Now;
                details.IsDeleted = false;
                details.IsActive = true;

                _dbContext.AdminDetails.Add(details);
                _dbContext.SaveChanges();


                return RedirectToAction("AdminLogin");

            }

            return View(admin);
        }

        [HttpGet]
        public IActionResult AdminLogout()
        {
            HttpContext.Session.Remove("AdminEmail");
            HttpContext.Session.Clear();
            return RedirectToAction("AdminLogin");
        }

        //Qr Code Trial
        public IActionResult CreateQRCode()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateQRCode(GenerateQRCodeModel generateQRCode)
        {
            try
            {
                GeneratedBarcode barcode = QRCodeWriter.CreateQrCode(generateQRCode.QRCodeText, 200);
                barcode.AddBarcodeValueTextBelowBarcode();
                // Styling a QR code and adding annotation text
                barcode.SetMargins(10);
                barcode.ChangeBarCodeColor(Color.BlueViolet);
                string path = Path.Combine(Environment.WebRootPath, "GeneratedQRCode");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string filePath = Path.Combine(Environment.WebRootPath, "GeneratedQRCode/qrcode.png");
                barcode.SaveAsPng(filePath);
                string fileName = Path.GetFileName(filePath);
                string imageUrl = "{this.Request.Scheme}://{this.Request.Host}{this.Request.PathBase}" + "/GeneratedQRCode/" + fileName;
                ViewBag.QrCodeUri = imageUrl;
            }
            catch (Exception)
            {
                throw;
            }
            return View();
        }
    }
}
