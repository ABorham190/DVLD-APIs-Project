using DVLDdataAccessLayer;
using Serilog;
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

       public async Task<bool> AddNewTakenTest()
        {
            this.TestID =await clsTestsDataLayer.AddNewTakenTest(this.TestAppointmentID,
                this.TestResult, this.Notes, this.CreatedByUserID);

            return this.TestID > 0;
        }
        public static int GetNumberOfFailedTests(int LDLAppID,int TestTypeID)
        {
            return clsTestsDataLayer.GetNumberOfFailedTests(LDLAppID, TestTypeID);
        }


        public static async Task< bool> TakeTest(TakeTestDto takenTest)
        {
            Log.Information("start executing TakeTest func in clsTests");

            clsAppointments appointment = clsAppointments.Find(takenTest.AppointmentID);

            if (appointment == null || appointment.IsAppointmentLocked)
            {
                Log.Error("appointment is null or locked");
                return false;
            }
            Log.Information($"Appointment is found with ID : {appointment.AppointmentID}");
            clsTests test = new clsTests
            {
                TestAppointmentID = takenTest.AppointmentID,
                TestResult = takenTest.IsSucceed,
                Notes = takenTest.Notes,
                CreatedByUserID = 1
            };

            if (!await test.AddNewTakenTest())
            {
                Log.Error("Test Not Added Successfully");
                return false;
            }
            Log.Information($"Test Added Successfully with ID {test.TestID}");

            if (clsAppointments.LockAppointment(appointment.AppointmentID))
                Log.Information("Appointment Locked Successfully");
            else
                Log.Error("Appointment Not Locked Successfully");

            return true;
        }

    }
}
