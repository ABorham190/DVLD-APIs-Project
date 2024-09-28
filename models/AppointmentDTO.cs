using System.ComponentModel.DataAnnotations;

namespace dvld_api.models
{
    public class AppointmentDTO
    {
        [Required(ErrorMessage ="LDLAppID required")]
        [RegularExpression(@"^[0-9]{2,10}",ErrorMessage ="LDLAppID must contain only numbers from 1 to 9 with length from 2 to 10")]
        public int LDLAppID { get; set; }

        [Required(ErrorMessage ="TestTypeID required")]
        [RegularExpression(@"^[1-3]{1}",ErrorMessage ="TestTypeID must contain only one number from 1 to 3 ")]
        public int TestTypeID { get; set; }
        
        public int CreatedByUserID { get { return 1; }  }

        [AppointmentDate(30)]
        public DateTime AppointmentDate { get; set; }
        //Decimal PaidFees { get; set; }
         public int RetakeTestAppID { get { return 0; } }
         public bool IsLocked { get { return false; } }
        public bool IsPassed { get { return false; } }

       
        
    }
}
