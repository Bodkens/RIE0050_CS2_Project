using CureConsultation.Models;
using DatabaseLayer;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;   
namespace CureConsultation.Controllers
{
    
    public class HomeController : Controller
    {
        
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            
        }

        
        public IActionResult Index([FromServices] LoginService loginService, [FromServices] AppointmentService appointmentService)
        {

            int? userId = HttpContext.Session.GetInt32("CurrentUserId");

            if (userId == null)
            {
                HttpContext.Session.SetInt32("LoginFailed", 0);
                return RedirectToAction("Index", "Login");
            }
            
            if (userId == 0)
            {
                HttpContext.Session.SetInt32("LoginFailed", 1);
                return RedirectToAction("Index", "Login");
            }

            HttpContext.Session.SetInt32("LoginFailed", 0);

            ViewBag.Appointments = appointmentService.LoadAppointments(userId.Value);

            return View();
        }

        public IActionResult UnassignAppointment(int id)
        {
            int? userId = HttpContext.Session.GetInt32("CurrentUserId");
            if (userId == null || userId == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            DatabaseManager.ExecuteNonQuerry("UPDATE Appointment SET userId = NULL WHERE Id = @id AND userId = @uid", 
                new Dictionary<string, object?>() { { "@id", id}, { "@uid", userId} });
            
            return RedirectToAction("Index", "Home");
        }

        public IActionResult AssignAppointment(int id)
        {
            int? userId = HttpContext.Session.GetInt32("CurrentUserId");
            if (userId == null || userId == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            DatabaseManager.ExecuteNonQuerry("UPDATE Appointment SET userId = @uid WHERE Id = @id",
                new Dictionary<string, object?>() { { "@id", id }, { "@uid", userId } });

            return RedirectToAction("Index", "Home");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}