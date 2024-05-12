using IoTBay.web.Data;
using IoTBay.web.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using IoTBay.web.ViewModels;

namespace IoTBay.web.Controllers
{
    public class OrderController : Controller
    {

        private readonly ApplicationDBContext _context;

        public OrderController(ApplicationDBContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        // GET: /OrderList/Index
        public IActionResult OrderList()
        {
            var orders = _context.OrderDetails
                .Include(od => od.Order)
                .Select(od => new OrderListViewModel
                {
                    OrderId = od.OrderId,
                    OrderDate = od.Order.OrderDate,
                    OrderDetails = new List<OrderDetailViewModel>  // Initialize OrderDetails
                    {
                        new OrderDetailViewModel
                        {
                            ProductName = od.Product.Name,
                            Price = od.Price
                        }
                    },
                    UserId = od.Order.UserId,
                    Quantity = od.Quantity
                })
                .ToList();
        
            return View("_OrderList", orders);
        }

        [HttpGet]
        public IActionResult Order()
        {
            ViewBag.Products = _context.Products.ToList();
            return View("_Order");
        }

        [HttpPost]
        public IActionResult SubmitOrder(int productId, int quantity)
        {
            var userId = User.Claims.FirstOrDefault(c => c.Type == "Id")?.Value;
            if (int.TryParse(userId, out int parsedUserId))
            {
                var order = new Order
                {
                    UserId = parsedUserId,
                    OrderDate = DateTime.Now,
                    Status = "Pending"
                };
                _context.Orders.Add(order);
                _context.SaveChanges();

                // Now create the associated OrderDetail for this Order
                var orderDetail = new OrderDetail
                {
                    OrderId = order.OrderId,
                    ProductId = productId,
                    Quantity = quantity,
                    // Calculate price based on the product's actual price (retrieve from the database)
                    Price = _context.Products.FirstOrDefault(p => p.ProductId == productId)?.Price ?? 0
                };
                _context.OrderDetails.Add(orderDetail);
                _context.SaveChanges();
            }

            return RedirectToAction("OrderList");
        }

        public IActionResult Edit(int id)
        {
            var order = _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefault(o => o.OrderId == id);

            if (order == null)
            {
                return NotFound();
            }

            // Pass the order to the Edit view for editing
            return View(order);
        }

        [HttpPost]
        public IActionResult Update(Order order)
        {
            if (ModelState.IsValid)
            {
                // Update the order in the database
                _context.Orders.Update(order);
                _context.SaveChanges();
                return RedirectToAction("OrderList");
            }

            // If model state is invalid, return to the edit view with validation errors
            return View("Edit", order);
        }
    }
}