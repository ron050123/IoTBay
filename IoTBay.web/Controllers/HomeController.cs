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

public class HomeController : Controller
{
    private readonly ApplicationDBContext _context;

    private readonly ILogger<HomeController> _logger;
    
    private readonly IEmailSender _emailSender;

    public HomeController(ILogger<HomeController> logger, ApplicationDBContext context, IEmailSender emailSender)
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
    
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(Usr model)
    {
            var user = _context.Usrs.FirstOrDefault(u => u.Email == model.Email && u.Password == model.Password);

            if (user == null)
            {
                ModelState.AddModelError(String.Empty, "Invalid email/password.");
                return View(model);
            }
            
            if (user.Role == "Deactivated")
            {
                ModelState.AddModelError(string.Empty, "This account is already deactivated.");
                return View(model);
            }
            
            if (!user.EmailConfirmed)
            {
                ModelState.AddModelError(string.Empty, "Email not verified.");
                return View(model);
            }
            
            var log = new AccessLog()
            {
                    UserId = user.UserId,
                    LoginTime = DateTime.Now
            };
            _context.AccessLogs.Add(log);
            await _context.SaveChangesAsync();
                
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.FullName),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("Id", user.UserId.ToString()) 
            };

            var identity = new ClaimsIdentity(claims, "CookieAuth");
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync("CookieAuth", principal);

            return RedirectToAction("Index");
            
    }

    [HttpGet]
    public IActionResult Register()
    {
        return View("~/Views/Home/Register.cshtml");
    }

    [HttpPost]
    public async Task<IActionResult> Register(Usr model)
    {
        var userExist = await _context.Usrs.FirstOrDefaultAsync(u => u.Email == model.Email);
        //make sure that no duplicate email account
        if (userExist != null)
        {
            ModelState.AddModelError("Email", "An account with this email already exists.");
            return View(model);
        }
        
        var user = new Usr
        {   
            FullName = model.FullName,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber,
            Password = model.Password,
            Role = "Customer",
            EmailConfirmed = false
        };
        //this for generate random code
        user.GenerateVerificationCode();
    
        try
        {
            _context.Add(user);
            await _context.SaveChangesAsync();
            
            var storedUser = await _context.Usrs.FirstOrDefaultAsync(u => u.Email == user.Email);
            if (storedUser == null)
            {
                ModelState.AddModelError("", "An error occurred. Please try again.");
                return View(model);
            }
            // for debugging too :(
            _logger.LogInformation("Verification code retrieved from: {Email} is {VerificationCode}", storedUser.Email, storedUser.VerificationCode);
            
            await _emailSender.SendEmailAsync(storedUser.Email, "Verify Your Email", $"Your verification code is: {storedUser.VerificationCode}");

            TempData["Email"] = storedUser.Email;
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", "An error occurred during registration. Please try again.");
            return View(model);
        }

        return RedirectToAction("VerifyCode");
    }
    
    [HttpGet]
    public IActionResult VerifyCode()
    {
        var viewModel = new VerificationViewModel
        {
            Email = TempData.Peek("Email")?.ToString()
        };
        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> VerifyCode(VerificationViewModel model)
    {
        _logger.LogInformation("Starting verification process for email: {Email}", model.Email);

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _context.Usrs.FirstOrDefaultAsync(u => u.Email == model.Email);
        if (user == null)
        {
            ModelState.AddModelError("", "No user associated with this email.");
            return View(model);
        }

        if (user.VerificationCode == model.VerificationCode)
        {
            user.EmailConfirmed = true;
            _context.Update(user);
            await _context.SaveChangesAsync();
            return RedirectToAction("RegistrationSuccess");
        }
        else
        { //for debug becasue keep messing up the code
            _logger.LogWarning("wrong code. Expected: {ExpectedCode}, Received: {ReceivedCode}", user.VerificationCode, model.VerificationCode);
            ModelState.AddModelError("", "Invalid verification code.");
            return View(model);
        }
    }
    
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        var userIdString = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;

        if (!int.TryParse(userIdString, out int userId))
        {
            return RedirectToAction("Login");
        }
        
        var log = await _context.AccessLogs
            .Where(l => l.UserId == userId && l.LogoutTime == null)
            .FirstOrDefaultAsync();
        
        if (log != null)
        {
            log.LogoutTime = DateTime.Now;
            _context.Update(log);
            await _context.SaveChangesAsync();
        }
        await HttpContext.SignOutAsync("CookieAuth");
        return RedirectToAction("Login");
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
            FullName = user.FullName,
            PhoneNumber = user.PhoneNumber
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

        return View(user);
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

        user.FullName = model.FullName;
        user.PhoneNumber = model.PhoneNumber;

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
            user.Role = "Deactivated";
            _context.Update(user);
            await _context.SaveChangesAsync();
            
            //this bring the user back to login!
            await HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }
        return View("Error");
    }
}