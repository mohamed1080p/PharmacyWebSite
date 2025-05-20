namespace PharmacyWebSite.Services
{
    public class AppConfigService
    {
        public string PharmacyName { get; set; }
        public string SupportEmail { get; set; }

        public AppConfigService()
        {
            PharmacyName = "My Pharmacy";
            SupportEmail = "support@pharmacy.com";
        }
    }
}
