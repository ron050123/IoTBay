using System.Diagnostics;
using IoTBay.web.Data;
using Microsoft.AspNetCore.Mvc;
using IoTBay.web.Models;
using IoTBay.web.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace IoTBay.web.Controllers;

public class HomeController : Controller
{
    private readonly ApplicationDBContext _context;

    private readonly ILogger<HomeController> _logger;

    public HomeController(ILogger<HomeController> logger, ApplicationDBContext context)
    {
        _logger = logger;
        _context = context;
    }
    public async Task<IActionResult> Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Id,Name,Price")] Product product)
    {
        if (ModelState.IsValid)
        {
            _context.Add(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View();
    }
    
    // GET: Products/Create
    public IActionResult Create()
    {
        return View();
    }

    public IActionResult Login()
    {
        return View();
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(Usr model)
    {
            var user = _context.Usrs.FirstOrDefault(u => u.Email == model.Email && u.Password == model.Password && u.IsActive == true);
            
            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim("Id", user.UserId.ToString()),
                    new Claim(ClaimTypes.Role, user.Role.ToString())

                };

                var identity = new ClaimsIdentity(claims, "CookieAuth");
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("CookieAuth", principal);

                return RedirectToAction("Index");
            }

            ModelState.AddModelError(string.Empty, "Invalid email or password.");
        
        return View(model);
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View("~/Views/Home/Register.cshtml");
    }

    [HttpPost]
    public async Task<IActionResult> Register(Usr model)
    {
        var user = new Usr
        {
            Email = model.Email,
            Password = model.Password,
            Role = "Customer",
            Phone = "",
            Name = "",
            Address = "",
            Type = "",
            IsActive = true
        };
        try
        {
            _context.Add(user);
            await _context.SaveChangesAsync();
            return View("RegistrationSuccess");
        }
        catch(Exception ex)
        {
            return View("~/Views/Home/Register.cshtml", user);
        }
    }
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync("CookieAuth");
        return RedirectToAction("Login");
    }

}