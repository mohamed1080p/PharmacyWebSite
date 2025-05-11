namespace PharmacyWebSite.Models
{
    public class Prescription
    {
        public int PrescriptionId { get; set; }
        public DateTime DateIssued { get; set; }
        public string DoctorName { get; set; }
        public string ImageUrl { get; set; }  // Link to uploaded prescription image

        // Foreign Key to User (the user who uploaded the prescription)
        public int UserId { get; set; }
        public User User { get; set; }

        // Relationships
        public ICollection<MedicinePrescription> MedicinePrescriptions { get; set; }
    }

}
