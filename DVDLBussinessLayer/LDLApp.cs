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

namespace DVDLBussinessLayer
{
    public class LDLApp
    {
       public int LDLAppID {  get; set; } 
       public int AppID { get; set; }
       public int LicenseTypeID { get; set; }

        enum enMode { AddNewMode = 0,UpdateMode=1 }
        enMode _Mode;

        private LDLApp (int lDLAppID, int appID, int licenseTypeID)
        {
            LDLAppID = lDLAppID;
            AppID = appID;
            LicenseTypeID = licenseTypeID;
            _Mode = enMode.UpdateMode;
        }

        public LDLApp() 
        {
            this.LDLAppID = 0;
            this.AppID = 0;
            this.LicenseTypeID = 0;
            _Mode=enMode.AddNewMode;
         
        
        }

        private bool _AddNewLDLApp()
        {
            this.LDLAppID = LDLAppDataLayer.AddNewLDLApp(this.AppID, this.LicenseTypeID);
            return this.LDLAppID != -1;
        }


        public bool Save()
        {
            switch (_Mode)
            {
                case enMode.AddNewMode:

                    if (_AddNewLDLApp()) 
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
        public static  DataTable GetAllLDLApps()
        {
            return LDLAppDataLayer.GetAllLDLApps();
        } 

        public static bool GetLicenseType(int LDLAppID,ref string LicenseType)
        {
            return LDLAppDataLayer.GetLicenseTypeUsingLDLAppID(LDLAppID, ref LicenseType);
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
    }
}
