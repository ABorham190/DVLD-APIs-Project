using DVLDdataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVDLBussinessLayer
{
    public  class clsTests
    {
        public  int TestID { get; set; }
        public int TestAppointmentID { get; set; }
        public bool TestResult { get; set; }
        public string Notes { get; set; }
        public int CreatedByUserID { get; set; }

       public bool AddNewTakenTest()
        {
            this.TestID = clsTestsDataLayer.AddNewTakenTest(this.TestAppointmentID,
                this.TestResult, this.Notes, this.CreatedByUserID);

            return this.TestID > 0;
        }
        public static int GetNumberOfFailedTests(int LDLAppID,int TestTypeID)
        {
            return clsTestsDataLayer.GetNumberOfFailedTests(LDLAppID, TestTypeID);
        }




    }
}
