using Hotel_Management_MVC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hotel_Management_MVC.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class HotelsApiController : ControllerBase
    {
        private readonly AdvancedTestDbContext _dbContext;
        //Base _base = new Base();
        public HotelsApiController(AdvancedTestDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet]
        public ActionResult GetHotel()
        {
            return Ok();
        }


        [HttpPost]
        public ActionResult AddHotel(HotelDetail model)
        {
            
            var hotelExist = _dbContext.HotelDetails.Find(model.Email);

            if (hotelExist == null)
            {
                //HotelDetail hotel = new HotelDetail();
                //hotel.Id = Guid.NewGuid().ToString();
                //hotel.HotelName = model.HotelName;
                //hotel.Email = model.Email;
                //hotel.Password = Base.EncryptPassword(model.Password);
                //hotel.EmailVerified = false;
                //hotel.IsActive = true;
                //hotel.IsDeleted = false;
                //hotel.CreationDate = DateTime.Now;
                //hotel.SingleBedAc = model.SingleBedAc;
                //hotel.SingleBedNonAc = model.SingleBedNonAc;
                //hotel.DoubleBedAc = model.DoubleBedAc;
                //hotel.DoubleBedNonAc = model.DoubleBedNonAc;
                //hotel.MaharajaSuite = model.MaharajaSuite;
                //hotel.CoverImageUrl = model.CoverImageUrl;
                //hotel.SingleBedAcPrice = model.SingleBedAcPrice;
                //hotel.DoubleBedAcPrice = model.DoubleBedAcPrice;
                //hotel.SingleBedNonAcPrice = model.SingleBedNonAcPrice;
                //hotel.DoubleBedNonAcPrice = model.DoubleBedNonAcPrice;
                //hotel.SingleBedAcImgUrl = model.SingleBedAcImgUrl;
                //hotel.SingleBedNonAcImgUrl = model.SingleBedNonAcImgUrl;
                //hotel.DoubleBedAcImgUrl = model.DoubleBedAcImgUrl;
                //hotel.DoubleBedNonAcImgUrl = model.DoubleBedNonAcImgUrl;

                _dbContext.HotelDetails.Add(model);
                _dbContext.SaveChanges();

                return StatusCode(201);
            }
            else
            {
                return StatusCode(208);
            }
            return Ok();
        }
    }
}
