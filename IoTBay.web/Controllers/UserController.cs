using Microsoft.AspNetCore.Mvc;
using IoTBay.web.Models.Entities;
using IoTBay.web.Data;

namespace IoTBay.web.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDBContext _context;

        public UserController(ApplicationDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View("~/Views/Home/Register.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> Register(Usr user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return Content("User registered successfully!");
            }
            else
            {
                return View("~/Views/Home/Register.cshtml", user);
            }
        }
    }
}