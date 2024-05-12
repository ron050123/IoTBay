namespace IoTBay.web.ViewModels
{
    public class OrderListViewModel
    {

        public OrderListViewModel()
        {
            OrderDetails = new List<OrderDetailViewModel>();
        }
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }

        public List<OrderDetailViewModel> OrderDetails { get; set; }

        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int UserId { get; set; }
        public int Quantity { get; set; }
    }

    public class OrderDetailViewModel
    {
        public string ProductName { get; set; }
        public decimal Price { get; set; }

    }
}