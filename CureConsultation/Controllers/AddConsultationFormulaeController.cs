using CureConsultation.Models;
using DatabaseLayer;
using Microsoft.AspNetCore.Mvc;

namespace CureConsultation.Controllers
{
    public class AddConsultationFormulaeController : Controller
    {
        
        public IActionResult Index(int id)
        {

            if(HttpContext.Session.GetInt32("CurrentUserId") == null || HttpContext.Session.GetInt32("CurrentUserId") == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewData["id"] = id;
            
            return View();
        }

        [HttpPost]
        public IActionResult Index(AddConsultationFormulaeModel formulaeModel)
        {
            if (HttpContext.Session.GetInt32("CurrentUserId") == null || HttpContext.Session.GetInt32("CurrentUserId") == 0)
            {
                return RedirectToAction("Index", "Home");
            }

            if (formulaeModel.ConsultationId != null && formulaeModel.ConsultationId != "Select consultation")
            {
                return RedirectToAction("Index", "AddConsultationFormulae", new { id = int.Parse(formulaeModel.ConsultationId.Split()[0]) });
            }

            return RedirectToAction("Index", "AddConsultationFormulae", new { id = 0 });

        }


    }
}
