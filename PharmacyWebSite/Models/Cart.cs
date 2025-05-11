namespace PharmacyWebSite.Models
{
    public class Cart
    {
        public int CartId { get; set; }
        public decimal TotalPrice { get; set; }

        // Foreign Key to User
        public int UserId { get; set; }
        public User User { get; set; }

        // Relationships
        public ICollection<CartItem> CartItems { get; set; }
    }

}
