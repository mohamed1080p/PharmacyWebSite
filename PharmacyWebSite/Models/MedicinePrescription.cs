namespace PharmacyWebSite.Models
{
    public class MedicinePrescription
    {
        public int Id { get; set; }  // Primary Key
        public int MedicineId { get; set; }  // Foreign Key to Medicine
        public int PrescriptionId { get; set; }  // Foreign Key to Prescription

        // Navigation properties
        public Medicine Medicine { get; set; }  // Navigation to Medicine
        public Prescription Prescription { get; set; }  // Navigation to Prescription
    }


}
