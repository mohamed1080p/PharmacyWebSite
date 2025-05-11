namespace PharmacyWebSite.Models
{
    public class Order
    {
        public int OrderId { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }  // Pending, Completed, etc.

        // Foreign Key to User (the customer who placed the order)
        public int UserId { get; set; }
        public User User { get; set; }

        // Relationships
        public ICollection<OrderItem> OrderItems { get; set; }
    }

}
