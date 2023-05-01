using CureConsultation.Models;
using DatabaseLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CureConsultation.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            ViewData["LoginFailed"] = HttpContext.Session.GetInt32("LoginFailed");
            return View();
          
        }

        [HttpPost]
        public IActionResult Index([FromServices] LoginService loginService, LoginForm loginForm)
        {
            if (ModelState.IsValid && loginForm.Email != null && loginForm.Password != null)
            {

                User? user = loginService.TryLogin(loginForm.Email, loginForm.Password);

                
                
                HttpContext.Session.SetInt32("CurrentUserId", user?.Id != null ? user.Id.Value : 0);
                

                return RedirectToAction("Index", "Home");
                
            }

            return View();
        }
    }
}
