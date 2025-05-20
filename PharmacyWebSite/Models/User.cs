namespace PharmacyWebSite.Models
{
    public interface IUserFactory
    {
        User Create(string name, string email, string password);
    }

    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsAdmin { get; set; }
        public string PhoneNumber { get; set; }

        public static class Factory
        {
            public static IUserFactory GetFactory(bool isAdmin)
            {
                return isAdmin ? new AdminUserFactory() :new CustomerUserFactory();
            }
        }

        private class AdminUserFactory : IUserFactory
        {
            public User Create(string name, string email, string password)
            {
                return new User
                {
                    Name = name,
                    Email = email,
                    Password = password,
                    IsAdmin = true
                };
            }
        }

        private class CustomerUserFactory : IUserFactory
        {
            public User Create(string name, string email, string password)
            {
                return new User
                {
                    Name = name,
                    Email = email,
                    Password=password,
                    IsAdmin = false
                };
            }
        }
    }
}