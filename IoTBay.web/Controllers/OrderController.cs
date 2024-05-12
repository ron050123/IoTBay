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

        public IActionResult Index(string searchString, DateTime? searchDate, int? searchOrderId)
        {
            IQueryable<Order> orders = _context.Orders;

            if (!string.IsNullOrEmpty(searchString))
            {
                orders = orders.Where(o => o.UserId.ToString().Contains(searchString));
            }

            if (searchDate.HasValue)
            {
                orders = orders.Where(o => o.OrderDate.Date == searchDate.Value.Date);
            }

            if (searchOrderId.HasValue)
            {
                orders = orders.Where(o => o.OrderId == searchOrderId.Value);
            }

            orders = orders.Include(o => o.OrderDetails);

            return View(orders.ToList());
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
        public IActionResult Update(int orderId, int productId, int quantity)
        {
            var order = _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefault(o => o.OrderId == orderId);

            if (order == null)
            {
                return NotFound();
            }

            // Update the OrderDetail for the specified product (assuming one product per order for simplicity)
            var orderDetail = order.OrderDetails.FirstOrDefault();
            if (orderDetail != null)
            {
                orderDetail.ProductId = productId;
                orderDetail.Quantity = quantity;
                _context.SaveChanges();
            }

            return RedirectToAction("OrderList");
        }
    }
}