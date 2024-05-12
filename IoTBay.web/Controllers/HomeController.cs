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
using Microsoft.Extensions.Logging;

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
    
    [Authorize]
    public async Task<IActionResult> Index()
    {
        var products = await _context.Products.ToListAsync(); 
        return View(products);
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
        if (ModelState.IsValid)
        {
            var user = _context.Usrs.FirstOrDefault(u => u.Email == model.Email && u.Password == model.Password);
            
            if (user != null)
            {
                var claims = new List<Claim>
                {
                    new Claim("Id", user.Id.ToString()) // Storing user ID
                };

                var identity = new ClaimsIdentity(claims, "CookieAuth");
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync("CookieAuth", principal);

                return RedirectToAction("Index");
            }

            ModelState.AddModelError(string.Empty, "Invalid email or password.");
        }


        // If model state is not valid, return the view with the model
        return View(model);
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
            return View("RegistrationSuccess");
        }
        else
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

    [Authorize]
    [HttpGet]
    public IActionResult OrderManagement()
    {
        return View();
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> OrderManagement(Order order)
    {
        if (ModelState.IsValid)
        {
            // Associate the order with the logged-in user
            var userId = User.FindFirstValue("Id");
            if (int.TryParse(userId, out int parsedUserId)) // Try parsing userId to int
            {
                order.UserId = parsedUserId;

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index"); // Redirect to a relevant page after order submission
            }
            else
            {
                // Handle the case where userId cannot be parsed to int
                ModelState.AddModelError(string.Empty, "Invalid user ID format.");
            }
        }
    return View(order);
}

}