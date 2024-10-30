namespace dvld_api.models
{
    public class clsApplicationDTO
    {
        public int ApplicationID { get; set; }
        public int ApplicantID { get; set; }
        public DateTime ApplicationDate { get; set; }
        public int ApplicationTypeID { get; set; }

        public byte ApplicationStatus { get; set; }
    }
}
