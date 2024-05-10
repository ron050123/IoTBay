[ApiController]
[Route("api/orders")]
public class OrderController : ControllerBase
{
    private readonly AppDbContext _dbContext;

    public OrderController(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    [HttpPost]
    public IActionResult CreateOrder(Order order)
    {
        // Validate order data
        if (order == null || string.IsNullOrEmpty(order.OrderStatus))
        {
            return BadRequest("Invalid order data");
        }

        // Save order to database
        _dbContext.Orders.Add(order);
        _dbContext.SaveChanges();

        return Ok("Order created successfully");
    }

    [HttpGet("{orderId}")]
    public IActionResult GetOrderById(int orderId)
    {
        var order = _dbContext.Orders.FirstOrDefault(o => o.OrderId == orderId);
        if (order == null)
        {
            return NotFound("Order not found");
        }

        return Ok(order);
    }

    [HttpPut("{orderId}")]
    public IActionResult UpdateOrderStatus(int orderId, string newStatus)
    {
        var order = _dbContext.Orders.FirstOrDefault(o => o.OrderId == orderId);
        if (order == null)
        {
            return NotFound("Order not found");
        }

        order.OrderStatus = newStatus;
        _dbContext.SaveChanges();

        return Ok("Order status updated successfully");
    }

    // Other CRUD operations can be similarly implemented
}