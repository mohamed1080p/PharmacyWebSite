using System.ComponentModel.DataAnnotations;

namespace PharmacyWebSite.Models
{
    public class Medicine
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string Description { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }

        public string Category { get; set; }

        [Range(0, int.MaxValue)]
        public int Stock { get; set; }

        public string? ImagePath { get; set; } // Optional for images

        // Relationships
        public ICollection<MedicinePrescription> MedicinePrescriptions { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; }
        public ICollection<CartItem> CartItems { get; set; }
    }

}
