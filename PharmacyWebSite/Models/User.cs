// Models/User.cs
namespace PharmacyWebSite.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsAdmin { get; set; }
        public ICollection<Order> Orders { get; set; }
        public ICollection<Prescription> Prescriptions { get; set; }
        public ICollection<Cart> Carts { get; set; }
    }
}