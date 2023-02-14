using Hotel_Management_MVC.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Hotel_Management_MVC.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QRCodeApiController : ControllerBase
    {
        [HttpPost]
        public IActionResult AddHotel(string email, string password)
        {
            //if(email != null && password != null) 
            //{
            //    return RedirectToAction("Index", "Dashboards");
            //}
            //else
            //{
            //    return BadRequest();
            //}
            return BadRequest();
        }
        
    }
}
