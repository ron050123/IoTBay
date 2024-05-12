using System.Diagnostics;
using IoTBay.web.Data;
using Microsoft.AspNetCore.Mvc;
using IoTBay.web.Models;
using IoTBay.web.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;

namespace IoTBay.web.Controllers;

public class OnlineUserAccessManagement : Controller
{
    private readonly ApplicationDBContext _context;

    private readonly ILogger<HomeController> _logger;

    private readonly IEmailSender _emailSender;

    public OnlineUserAccessManagement(ILogger<HomeController> logger, ApplicationDBContext context,
        IEmailSender emailSender)
    {
        _logger = logger;
        _context = context;
        _emailSender = emailSender;
    }
    
    public async Task<IActionResult> Index()
    {
        var products = await _context.Products.ToListAsync(); 
        return View(products);
    }

    public IActionResult Login()
    {
        return View();
    }
    
    public IActionResult RegistrationSuccess()
    {
        return View();
    }
    
    public async Task<IActionResult> LogHistory()
    {
        if (!User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Login");
        }
        var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == "Id");
        if (userIdClaim == null)
        {
            return View("Error");
        }

        if (!int.TryParse(userIdClaim.Value, out int userId))
        {
            return View("Error");
        }
        
        var log = await _context.AccessLogs
            .Where(l => l.UserId == userId)
            .ToListAsync();
        return View(log);
    }

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> EditProfile()
    {
        var userIdString = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;

        if (!int.TryParse(userIdString, out int userId))
        {
            return RedirectToAction("Login");
        }

        var user = await _context.Usrs.FindAsync(userId);
        if (user == null)
        {
            return View("Error");
        }

        var model = new UpdatePorfileModel()
        {
            Name = user.Name,
            PhoneNumber = user.Phone
        };

        return View(model);
    }

    [HttpGet]
    public IActionResult Porifle()
    {
        var userId = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
        if (!int.TryParse(userId, out int userIdInt))
        {
            return RedirectToAction("Login");
        }

        var user = _context.Usrs.Find(userIdInt);
        if (user == null)
        {
            return View("Error");
        }

        return View("Porfile", user);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProfile(UpdatePorfileModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var userId = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
        if (!int.TryParse(userId, out int userIdInt))
        {
            return RedirectToAction("Login");
        }

        var user = await _context.Usrs.FindAsync(userIdInt);
        if (user == null)
        {
            return View("Error");
        }

        user.Name = model.Name;
        user.Phone = model.PhoneNumber;

        try
        {
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "An error occurred: " + ex.Message);
            return View(model);
        }
    }
    
    [Authorize]
    public async Task<IActionResult> DeactivateAcc()
    {
        var userIdString = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
        if (!int.TryParse(userIdString, out int userId))
        {
            return RedirectToAction("Login");
        }
    
        var user = await _context.Usrs.FindAsync(userId);
        if (user != null)
        {
            user.IsActive = false; 
            _context.Update(user);
            await _context.SaveChangesAsync();
            
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }
        return View("Error");
    }
    
    
}


