using DVLDdataAccessLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DVDLBussinessLayer
{
    public class clsDriver
    {
        public int DriverID;
        public int PersonID;
        public int CreatedByUserID;
        public DateTime CreatedDate;

        public  bool AddNewDriver()
        {
            this.DriverID = clsDriversDataLayer.AddNewDriver(this.PersonID,
                this.CreatedByUserID, this.CreatedDate);
            return this.DriverID != 0;
        }
        public static DataTable GetAllDriversDetails()
        {
            return clsDriversDataLayer.GetManageDriversDetails();
        }
    }
}
