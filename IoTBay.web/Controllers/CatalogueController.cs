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

public class CatalogueController : Controller
{
    private readonly ApplicationDBContext _context;
    private readonly ILogger<CatalogueController> _logger;   
    
    public CatalogueController(ILogger<CatalogueController> logger, ApplicationDBContext context)
    {
        _logger = logger;
        _context = context;
    }
    
    public async Task<IActionResult> Index()
    {
      return View();
    }
    
    public async Task<IActionResult> GetCatalogue(string search,decimal maxPrice)
    {
        IQueryable<Product> query = _context.Products;

        if (!string.IsNullOrEmpty(search))
        {
            string lowerSearch = search.ToLower();
            query = query.Where(p => p.Name.ToLower().Contains(lowerSearch)); 
        }

        query = query.Where(p => p.Price <= maxPrice).OrderByDescending(p => p.ProductId );
        
        var products = await query.ToListAsync();
        return PartialView("_Catalogue",products);
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> CreateProduct([Bind("Name,Description,Price")] Product product)
    {
        if (ModelState.IsValid)
        {
            _context.Add(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return PartialView("_Create", product);
    }
    [Authorize]
    public IActionResult CreateProduct()
    {
        return PartialView("_Create");
    }
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> UpdateProduct([Bind("ProductId,Name,Description,Price")] Product product)
    {
        if (ModelState.IsValid)
        {
            var existingProduct = await _context.Products.FindAsync(product.ProductId);

            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.Price = product.Price;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return PartialView("_Update");
    }
    [Authorize]
    public async Task<IActionResult>  UpdateProduct(int id)
    {
        var existingProduct =  await _context.Products.FindAsync(id);
        
        if (existingProduct != null)
        {
            return PartialView("_Update",existingProduct);
        }
        return NotFound();
        
    }
    
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> ConfirmDeleteProduct(int ProductId)
    {

            var existingProduct = await _context.Products.FindAsync(ProductId);
            if (existingProduct != null)
            {
                _context.Remove(existingProduct);
                await _context.SaveChangesAsync();  
                return RedirectToAction(nameof(Index));

            }

            return NotFound();
    }
    [Authorize]
    public async Task<IActionResult>  DeleteProduct(int id)
    {
        var existingProduct =  await _context.Products.FindAsync(id);
        
        if (existingProduct != null)
        {
            return PartialView("_Delete",existingProduct);
        }
        return NotFound();
        
    }

}
