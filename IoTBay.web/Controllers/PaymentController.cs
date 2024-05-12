﻿using System.Diagnostics;
using IoTBay.web.Data;
using Microsoft.AspNetCore.Mvc;
using IoTBay.web.Models;
using IoTBay.web.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Claims;
using IoTBay.web.Migrations;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace IoTBay.web.Controllers;

public class PaymentController : Controller
{
    private readonly ApplicationDBContext _context;
    private readonly ILogger<PaymentController> _logger;

    public PaymentController(ILogger<PaymentController> logger, ApplicationDBContext context)
    {
        _logger = logger;
        _context = context;
    }

    // GET: Payment/Create
    [Authorize]
    public IActionResult Create()
    {
        List<string> paymentMethods = new List<string> { "Visa", "MasterCard", "PayPal", "Bank Transfer/Cheque", "Cash on Pickup", "Apple Pay" };
        ViewBag.PaymentMethods = new SelectList(paymentMethods);
        
        List<string> status = new List<string> { "Complete", "Pending", "Cancelled" };
        ViewBag.Status = new SelectList(status);
        
        Payment payment = new Payment
        {
            UserId = 1,
            OrderId = 1 
        };

        return View(payment);
    }


    // POST: Payment/Create
    [HttpPost, ActionName("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("TransactionDate,Amount,PaymentMethod,Status,UserId,OrderId")] Payment payment)
    {
        List<string> paymentMethods = new List<string> { "Visa", "MasterCard", "PayPal", "Bank Transfer/Cheque", "Cash on Pickup", "Apple Pay" };
        ViewBag.PaymentMethods = new SelectList(paymentMethods);
        
        List<string> status = new List<string> { "Complete", "Pending", "Cancelled" };
        ViewBag.Status = new SelectList(status);

        payment.User = payment.UserId != 0 ? _context.Usrs.Find(payment.UserId) : null;

            ModelState.Remove("User");
            ModelState.Remove("Order");
        if (ModelState.IsValid)
        {
            _context.Add(payment);
            _context.Entry(payment).State = EntityState.Added;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        return View(payment);
    }



    // GET: Payment
    public async Task<IActionResult> Index(string searchString)
    {
        IQueryable<Payment> payment = _context.Payment;
        
        if (!String.IsNullOrEmpty(searchString))
        {
            payment = payment.Where(s => s.PaymentId.ToString().Contains(searchString)
                                         || s.TransactionDate.ToString().Contains(searchString)
                                         || s.Amount.ToString().Contains(searchString)
                                         || (s.PaymentMethod != null && s.PaymentMethod.Contains(searchString))
                                         || (s.Status != null && s.Status.Contains(searchString)));
        }

        return View(await payment.ToListAsync());
    }


    // GET: Payment/Edit/5
    public async Task<IActionResult> Edit(int? id)
    {
        List<string> paymentMethods = new List<string> { "Visa", "MasterCard", "PayPal", "Bank Transfer/Cheque", "Cash on Pickup", "Apple Pay" };
        ViewBag.PaymentMethods = new SelectList(paymentMethods);
        
        List<string> status = new List<string> { "Complete", "Pending", "Cancelled" };
        ViewBag.Status = new SelectList(status);
        
        if (id == null)
        {
            return NotFound();
        }

        var payment = await _context.Payment
            .FirstOrDefaultAsync(p => p.PaymentId == id);

        if (payment == null)
        {
            return NotFound();
        }
        return View(payment);
    }

// POST: Payment/Edit/5
    [HttpPost, ActionName("Edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("PaymentId,TransactionDate,Amount,PaymentMethod,Status")] Payment payment)
    {
        List<string> paymentMethods = new List<string> { "Visa", "MasterCard", "PayPal", "Bank Transfer/Cheque", "Cash on Pickup", "Apple Pay" };
        ViewBag.PaymentMethods = new SelectList(paymentMethods);
        
        List<string> status = new List<string> { "Complete", "Pending", "Cancelled" };
        ViewBag.Status = new SelectList(status);
        
        if (id != payment.PaymentId)
        {
            return NotFound();
        }

        var existingPayment = await _context.Payment
            .FirstOrDefaultAsync(p => p.PaymentId == id);

        if (existingPayment == null)
        {
            return NotFound();
        }
        ModelState.Remove("User");
        ModelState.Remove("Order");

        if (ModelState.IsValid)
        {
            existingPayment.TransactionDate = payment.TransactionDate;
            existingPayment.Amount = payment.Amount;
            existingPayment.PaymentMethod = payment.PaymentMethod;
            existingPayment.Status = payment.Status;

            try
            {
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!PaymentExists(payment.PaymentId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }
        if (!ModelState.IsValid)
        {
            foreach (var modelStateKey in ModelState.Keys)
            {
                var modelStateVal = ModelState[modelStateKey];
                foreach (var error in modelStateVal.Errors)
                {
                    _logger.LogError("Error in ModelState for key {Key}: {Error}", modelStateKey, error.ErrorMessage);
                }
            }
        }
        return View(payment);
    }




    private bool PaymentExists(int id)
    {
        return _context.Payment.Any(e => e.PaymentId == id);
    }

// GET: Payment/Delete/5
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var payment = await _context.Payment
            .FirstOrDefaultAsync(m => m.PaymentId == id);
        if (payment == null)
        {
            return NotFound();
        }

        return View(payment);
    }

// POST: Payment/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var payment = await _context.Payment.FindAsync(id);
        _context.Payment.Remove(payment);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

}