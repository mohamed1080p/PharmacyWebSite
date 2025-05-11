namespace PharmacyWebSite.Models
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }

        // Foreign Key to Order
        public int OrderId { get; set; }
        public Order Order { get; set; }

        // Foreign Key to Medicine
        public int MedicineId { get; set; }
        public Medicine Medicine { get; set; }
    }

}
