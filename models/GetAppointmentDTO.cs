namespace dvld_api.models
{
    public class GetAppointmentDTO
    {
        public int AppointmentID { get; set; }
        public int LDLAppID { get; set; }
        public int TestTypeID { get; set; }
        public bool IsAppointmentLocked { get; set; }
        public int CreatedByUserID { get; set; }
        public DateTime AppointmentDate { get; set; }
       // public Decimal PaidFees { get; set; }
        //public int RetakeTestAppID { get; set; }

        //public int IsPassed { get; set; }
       // public int IsLocked { get; set; }
    }
}
