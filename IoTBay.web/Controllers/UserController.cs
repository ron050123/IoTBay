﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using IoTBay.web.Models;
using Microsoft.AspNetCore.Identity;
using IoTBay.web.Models.Entities;
using IoTBay.web.Data;

namespace CarWorkshop.Controllers;

[Authorize(Roles = "Admin")] // restrict access to admin users only
public class UserController : Controller
{
	private readonly ApplicationDBContext _context;

	public UserController(ApplicationDBContext context)
	{
		_context = context;
	}

	public async Task<IActionResult> Index()
	{
		var users = _context.Usrs.ToList();
		return View(users);
	}

	// GET: Vendor/Edit/5
	public async Task<IActionResult> Edit(int? id)
	{
		if (id == 0 || id == null)
		{
			return View(new Usr());
		}

		var user = await _context.Usrs.FindAsync(id);
		return View(user);
	}

	[HttpPost]
	public async Task<IActionResult> SaveChanges([FromBody] Usr user)
	{
		if (user.Name != null && user.Password != null
		                      && user.Email != null && user.Phone != null && user.Role != null)
		{
			if (user.UserId == 0 || user.UserId == null)
			{

				// Attempt to create a new user
				_context.Usrs.Add(user);
				await _context.SaveChangesAsync();
				return Ok("New User saved successfully"); // Return success message
			}
			else
			{
				var existingUser = await _context.Usrs.Where(x => x.UserId == user.UserId).FirstOrDefaultAsync();

				existingUser.Name = user.Name;
				existingUser.Phone = user.Phone;
				existingUser.Email = user.Email;
				existingUser.Password = user.Password;
				existingUser.Role = user.Role;
				existingUser.IsActive = user.IsActive;

				// Attempt to update the user
				_context.Usrs.Update(existingUser);
				await _context.SaveChangesAsync();
				return Ok("User saved successfully"); // Return success message
			}
		}
		return BadRequest("Invalid data. Fill all fields"); // Return bad request if model state is invalid
	}

	[HttpGet]
	public IActionResult Create()
	{
		// Initialize a new user with default values
		var newUser = new Usr
		{
			IsActive = true, // Default as active
			EmailConfirmed = false, // Default as not confirmed
		};
		return View(newUser);
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
			IsActive = true,
			EmailConfirmed = false
		};
		user.GenerateVerificationCode();
            
		try
		{
			_context.Add(user);
			_context.SaveChangesAsync();
			return RedirectToAction("Index");
		}
		catch(Exception ex)
		{
			return View("~/Views/User/Create.cshtml", user);
		}
	}


	[HttpPost]
	public async Task<IActionResult> Delete(int id)
	{
		try
		{
			// Retrieve the user from the database
			var user = await _context.Usrs.FindAsync(id);
			if (user == null)
			{
				return NotFound();
			}

			// Remove the user from the database
			_context.Usrs.Remove(user);
			await _context.SaveChangesAsync();

			// Return success message
			return Ok("User deleted successfully");
		}
		catch (Exception ex)
		{
			// Return error message
			return BadRequest(ex.Message);
		}
	}
	
	[HttpPost]
	public async Task<IActionResult> ToggleStatus(int id)
	{
		var user = await _context.Usrs.FindAsync(id);
		if (user == null)
		{
			return NotFound("User not found.");
		}

		user.IsActive = !user.IsActive;  
		_context.Usrs.Update(user);       
		await _context.SaveChangesAsync();
		return Ok(user.IsActive ? "User activated." : "User deactivated.");
	}
	
	[HttpGet]
	public IActionResult GetUsers()
	{
		var users = _context.Usrs.Select(v => new
		{
			UserID = v.UserId,
			Name = v.Name
		}).ToList();

		return Json(users);
	}
}
