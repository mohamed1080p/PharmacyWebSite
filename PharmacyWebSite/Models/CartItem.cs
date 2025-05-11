namespace PharmacyWebSite.Models
{
    public class CartItem
    {
        public int CartItemId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        // Foreign Key to Cart
        public int CartId { get; set; }
        public Cart Cart { get; set; }

        // Foreign Key to Medicine
        public int MedicineId { get; set; }
        public Medicine Medicine { get; set; }
    }

}
