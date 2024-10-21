namespace dvld_api.models
{
    public class LicenseDTO
    {
        public int LicenseID { get; set; }
        public int ApplicationID { get; set; }
        public int DriverID { get; set; }
        public int LicenseClassID { get; set; }
        public DateTime IssueDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string Notes { get; set; } = null!;
        public Decimal PaidFees { get; set; }
        public bool IsActive {  get; set; }
        public byte IssueReason { get; set; }
        public int CreatedByUserID { get; set; }
        public string? ImagePath {  get; set; }
        public string ClassName { get; set; } = null!;
        public string NationalNo { get; set; } = null!;
        public bool IsDetained { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Name { get; set; }=null!;
        public byte Gender { get; set; }

        public string? ApplicantName { get; }


    }
}
