namespace PharmacyWebSite.Models
{
    public class MedicinePrescription
    {
        public int Id { get; set; }  
        public int MedicineId { get; set; }  
        public int PrescriptionId { get; set; }  

        public Medicine Medicine { get; set; }  
        public Prescription Prescription { get; set; }  
    }


}
