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
using System.Linq;

namespace IoTBay.web.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly ILogger<CustomerController> _logger;   
    
        public CustomerController(ILogger<CustomerController> logger, ApplicationDBContext context)
        {
            _logger = logger;
            _context = context;
        }
        
        public async Task<IActionResult> Index()
        {
            return View("~/Views/Customers/Index.cshtml");
        }
        
        public async Task<IActionResult> GetCustomers(string search, string type)
        {
            IQueryable<Usr> customers = _context.Usrs.Where(u => u.Role == "Customer");
            
            if (!string.IsNullOrEmpty(search))
            {
                string lowerSearch = search.ToLower();
                customers = _context.Usrs.Where(u => u.Name.ToLower().Contains(lowerSearch));
            }
            
            if (type != "All")
            {
                customers = customers.Where(u => u.Type == type);
            }
            
            customers = customers.OrderBy(u => u.Name);
            
            return PartialView("~/Views/Customers/_CustomerTable.cshtml", customers.ToList());
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View("~/Views/Customers/Create.cshtml");
        }

        [HttpPost]
        public IActionResult Create(Usr model)
        {
            var user = new Usr
            {
                Email = model.Email,
                Password = "",
                Role = "Customer",
                Phone = "",
                Name = model.Name,
                Address = model.Address,
                Type = model.Type,
                IsActive = true
            };
            try
            {
                _context.Add(user);
                _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                return View("~/Views/Customers/Create.cshtml", user);
            }
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var user = _context.Usrs.FirstOrDefault(u => u.UserId == id);
            if (user == null)
            {
                return NotFound();
            }
            return View("~/Views/Customers/Edit.cshtml", user);
        }

        [HttpPost]
        public IActionResult Edit(Usr model)
        {
            try
            {
                var existingUser = _context.Usrs.FirstOrDefault(u => u.UserId == model.UserId);
                if (existingUser == null)
                {
                    return NotFound();
                }
                
                existingUser.Email = model.Email;
                existingUser.Name = model.Name;
                existingUser.Address = model.Address;
                existingUser.Type = model.Type;
                existingUser.IsActive = model.IsActive;
                
                _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch(Exception ex)
            {
                return View("~/Views/Customers/Edit.cshtml", model);
            }
        }

        [HttpGet]
        public IActionResult Delete(int id)
        {
            var user = _context.Usrs.FirstOrDefault(u => u.UserId == id);
            if (user != null)
            {
                _context.Usrs.Remove(user);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}