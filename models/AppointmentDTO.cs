using DVDLBussinessLayer;
using System.ComponentModel.DataAnnotations;

namespace dvld_api.models
{
    public class AppointmentDTO
    {
        [Required(ErrorMessage ="LDLAppID required")]
        [Range(1,int.MaxValue,ErrorMessage ="LDLAppID must contain only numbers from 1 to 9 ")]
        public int LDLAppID { get; set; }

        [Required(ErrorMessage ="TestTypeID required")]
        [Range(1,3,ErrorMessage ="TestTypeID must contain only one number from 1 to 3 ")]
        public int TestTypeID { get; set; }
        
        public int CreatedByUserID { get { return 1; }  }

        [AppointmentDate(30)]
        public DateTime AppointmentDate { get; set; }
        public Decimal PaidFees { get { return clsTestTypes.GetTestFees(this.TestTypeID); }}
         public int RetakeTestAppID { get { return 0; } }
         public bool IsLocked { get { return false; } }
        public bool IsPassed { get { return false; } }

       
        
    }
}
