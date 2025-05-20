namespace PharmacyWebSite.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }  
        public int UserId { get; set; }
        public User User { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

        public class Builder
        {
            private readonly Order _order = new();

            public Builder ForUser(int userId)
            {
                _order.UserId = userId;
                _order.OrderDate = DateTime.Now;
                return this;
            }

            public Builder WithStatus(string status)
            {
                _order.Status = status;
                return this;
            }

            public Builder WithItems(IEnumerable<CartItem> cartItems)
            {
                _order.OrderItems = cartItems.Select(ci => new OrderItem
                {
                    MedicineId = ci.MedicineId,
                    Quantity = ci.Quantity,
                    Price = ci.Price
                }).ToList();

                _order.TotalPrice = _order.OrderItems.Sum(oi => oi.Price * oi.Quantity);
                return this;
            }

            public Order Build()
            {
                if (_order.UserId <= 0)
                    throw new InvalidOperationException("User ID is required");

                if (!_order.OrderItems.Any())
                    throw new InvalidOperationException("Order must contain items");

                return _order;
            }
        }
    }
}