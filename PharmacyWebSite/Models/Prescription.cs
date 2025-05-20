namespace PharmacyWebSite.Models
{
    public class Prescription
    {
        public int PrescriptionId { get; set; }
        public DateTime DateIssued { get; set; }
        public string DoctorName { get; set; }
        public string ImageUrl { get; set; }  

        
        public int UserId { get; set; }
        public User User { get; set; }

        public ICollection<MedicinePrescription> MedicinePrescriptions { get; set; }
    }

}
