using DVLDdataAccessLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Serilog;

namespace DVDLBussinessLayer
{
    public class LDLApp
    {
       public int LDLAppID {  get; set; } 
       public int AppID { get; set; }
       public int LicenseTypeID { get; set; }
        public clsOrders Application {  get; set; }
        public int PassedTests {  get; set; }
        


        enum enMode { AddNewMode = 0,UpdateMode=1 }
        enMode _Mode;

        private LDLApp (int lDLAppID, int appID, int licenseTypeID)
        {
            LDLAppID = lDLAppID;
            AppID = appID;
            LicenseTypeID = licenseTypeID;
            Application = clsOrders.FindApplicationByID(appID);

            _Mode = enMode.UpdateMode;
        }

        public LDLApp() 
        {
            this.LDLAppID = 0;
            this.AppID = 0;
            this.LicenseTypeID = 0;
            _Mode=enMode.AddNewMode;
        }

        private async Task<bool> _AddNewLDLApp()
        {
            this.LDLAppID =await LDLAppDataLayer.AddNewLDLApp(this.AppID, this.LicenseTypeID);
            return this.LDLAppID != -1;
        }


        public async Task <bool> Save()
        {
            switch (_Mode)
            {
                case enMode.AddNewMode:

                    if (await _AddNewLDLApp()) 
                    {
                        _Mode = enMode.UpdateMode;
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                    
        }

            return false;
        }
        public static async Task<  List<LDLAppDataLayer.LDLAppDTO>> GetAllLDLApps()
        {
            return await LDLAppDataLayer.GetAllLDLApps();
        } 

        public static async Task<string> GetLicenseTypeByLicenseClassID(int LicenseClassID)
        {
            return await LDLAppDataLayer.GetLicenseTypeUsingLicenseClassID(LicenseClassID);
        }

        public static LDLApp FindLDLApp(int LDLAppID)
        {
            int AppID = 0, LicenseTID = 0;
            if(LDLAppDataLayer.FindLDLApp(LDLAppID,ref AppID,ref LicenseTID))
            {
                return new LDLApp(LDLAppID, AppID, LicenseTID);
            }
            else
            {
                return null;
            }
        }
        //public static async Task<int>GetNumberOfPassedTestsForLDLApp(int LDLAppID)
        //{
        //    int NumberOfPassedTest = 0;
        //    bool IsSuccess=false;
        //    (NumberOfPassedTest,IsSuccess)= await LDLAppDataLayer.GetNumberOfPassedTestsForLDLApp(LDLAppID);
        //    if (IsSuccess)
        //    {
        //        return NumberOfPassedTest;
        //    }
        //    else { return -1; }
        //}
        enum enTests { Vision=1,Written=2,Street=3}
        public static async Task<bool> IsThisLDLAppIDAllowedToBookTest(int LDLAppID, int TestTypeID)
        {
            Log.Information($"Start IsThisLDLAppIDAllowedToBookTest (LDLApp) to check if {LDLAppID} Allowed to book test or not ");

            LDLApp ldlapp = LDLApp.FindLDLApp(LDLAppID);
            if (ldlapp == null)
            {
                Log.Error($"There is no LALApp with LDLAppID : {LDLAppID}");
                return false;
            }
            //Check if application status cancelled or completed
            if (ldlapp.Application.ApplicationStatus == 2 || ldlapp.Application.ApplicationStatus == 3)
            {
                Log.Error($"This LDLApp Is Follw Cancelled or Completed Application with ID : {ldlapp.Application.ApplicationID}");
                return false;
            }
            //you may delete this check , dont need it as you check in line 116 
            if (await clsAppointments.IsThisLDAppIDPassedTest(LDLAppID, TestTypeID))
            {
                Log.Error($"This LDLAppID is already passed {(enTests)TestTypeID} test");
                return false;
            }
            ldlapp.PassedTests = await LDLAppDataLayer.GetNumberOfPassedTestsForLDLApp(ldlapp.LDLAppID);

            if (TestTypeID <= ldlapp.PassedTests)
            {
                Log.Error($"This LDLApp is already pass this test");
                return false;
            }
            //set number of passed test properity to current ldlapp
            if (TestTypeID > ldlapp.PassedTests + 1)
            {
                Log.Error($"This LDLAppID must be pass {(enTests)(ldlapp.Application.PassedTests+1)} Test Firstly");
                return false;
            }


            int AppointmentID = 0;
            if(clsAppointments.IsThisLDAppIDHasAnyActiveAppointments(LDLAppID,TestTypeID,ref AppointmentID))
            {
                Log.Error($"This LDLApp already has an active appointment with appointmentID : {AppointmentID}");
                return false;
            }

            return true;


        }

        public static async Task<bool>IsLDLAppExists(int LDLAppID)
        {
            return await LDLAppDataLayer.IsLDLAppExists(LDLAppID);
        }
    }
}
