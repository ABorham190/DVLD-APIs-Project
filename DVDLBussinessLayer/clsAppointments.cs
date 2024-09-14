using DVLDdataAccessLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.SymbolStore;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using GlobalStructs;

namespace DVDLBussinessLayer
{
    public class clsAppointments
    {
        public int AppointmentID {  get; set; }
        public int DLAppID {  get; set; }
        public int TestTypeID { get; set; }
        public bool IsAppointmentLocked { get; set; }
        public int CreatedByUserID { get; set; }
        public DateTime AppointmentDate { get; set; }
        public Decimal PaidFees { get; set; }
        public  int RetakeTestAppID { get; set; }

        public int IsPassed { get; set; }
        public int IsLocked { get; set; }
        enum enMode { AddNew=0, Update=1 };
        enMode _Mode;

        public clsAppointments()
        {
           this. _Mode = enMode.AddNew;
           this.DLAppID = 0;
            this.TestTypeID = 0;
            this.IsPassed = 0;
            this.IsLocked = 0;
            this.AppointmentDate = DateTime.Now;
        }
        private clsAppointments(int appointmentid,int ldlappid,int testtypeid,
            bool isappointmentlocked,int createdbyuserid,DateTime appointmentdate,
            Decimal paidfees,int retaketestappid)
        {
           this.AppointmentID = appointmentid;
            this.DLAppID=ldlappid;
            this.AppointmentDate=appointmentdate;
            this.RetakeTestAppID = retaketestappid;
            this.CreatedByUserID = createdbyuserid;
            this.IsAppointmentLocked = isappointmentlocked;
            this.PaidFees = paidfees;
            this.TestTypeID=testtypeid;

            _Mode = enMode.Update;
        }
        public static DataTable GetVisionAppointmentsForDLAppID(int DLAppID)
        {
            return clsAppointmentsDataLayer.GetAllVisionTestAppointmentsforDLAppID(DLAppID);
        }

        public static clsAppointments Find(int AppointmentID)
        {
            int DLAppID = 0, TestTypeID = 0,LDLAppID =0,CreatedByUserID=0,
                RetakeTestApplicationID=0 ;
            DateTime AppointmentDate = DateTime.Now;
            Decimal PaidFees = 0;
            bool IsAppointmentLocked = false;
            if(clsAppointmentsDataLayer.FindAppointmentApp(AppointmentID,
                ref TestTypeID,ref LDLAppID,ref AppointmentDate,
                ref PaidFees,ref CreatedByUserID,ref IsAppointmentLocked,
                ref RetakeTestApplicationID))
            {
                return new clsAppointments(AppointmentID,DLAppID,
                    TestTypeID,IsAppointmentLocked,CreatedByUserID,
                    AppointmentDate,PaidFees,RetakeTestApplicationID);
            }
            else
            {
                return null;
            }

        }
        private bool _AddNewAppointment()
        {
            this.AppointmentID = clsAppointmentsDataLayer.AddNewAppointment(this.TestTypeID,
                this.DLAppID,this.AppointmentDate,this.PaidFees,this.CreatedByUserID
                ,this.IsAppointmentLocked, this.RetakeTestAppID);
            return this.AppointmentID != -1;
                
        }

        private bool _UpdateAppointment()
        {
            return clsAppointmentsDataLayer.UpdateAppointment
                (this.AppointmentID,  this.IsAppointmentLocked, this.AppointmentDate);
        }



        public bool Save()
        {
            switch (_Mode)
            {
                case enMode.AddNew:
                    if (_AddNewAppointment())
                    {
                        _Mode = enMode.Update;

                        return true;
                    }
                    else
                    {
                        return false;
                    }

                case enMode.Update:
                    if(_UpdateAppointment())
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
            }

            return false;
        }

        public static bool IsDLAppIDHasVisionAppointments(int DLAppID)
        {
            return clsAppointmentsDataLayer.IsThisDLAppIDHasAnyAppointments(DLAppID);
        }

        public static bool IsDLAppHasAnyActiveVissionAppointments(int DLAppID)
        {
            return clsAppointmentsDataLayer.IsThisDLAppIDHasAnyActiveVissionTestAppointment(DLAppID);
        }

        public static bool GetTakeTestDetails(int AppointmentID,ref GeneralStructs TakeTestStruct)
        {
            if(clsAppointmentsDataLayer.GetTakeTestDetails(AppointmentID,ref TakeTestStruct))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsThisDLAppIDPassVisionTest(int DLAppID)
        {
            return clsAppointmentsDataLayer.IsThisDLAppIDPassVisionTest(DLAppID);
        }

        public static bool IsThereVisionAppointmentsForThisDLAppID(int DLAppID)
        {
            return clsAppointmentsDataLayer.IsThereAreAnyVisionAppointmentsForThisDLAppID(DLAppID); 
        }

        public static bool LockAppointment(int AppointmentID)
        {
            return clsAppointmentsDataLayer.LockAnAppointment(AppointmentID);
        }

        public static DataTable GetAllWrittenTestAppointmentsForThisLDLAppID(int LDLAppID)
        {
            return clsAppointmentsDataLayer.GetAllWrittenTestAppointmentsForThisLDLAppID(LDLAppID);
        }
        public static bool IsThisLDAppIDPassedTest(int LDLAppID,int TestTypeID)
        {
            return clsAppointmentsDataLayer.IsThisLDLAppIDPassedTest(LDLAppID, TestTypeID);
        }
        public static bool IsThisLDAppIDHasAnyActiveAppointments(int LDLAppID,int TestTypeID,ref int AppointmentID)
        {
            return clsAppointmentsDataLayer.IsThisLDLAppIDHasAnyActiveAppointments(LDLAppID, TestTypeID, ref AppointmentID);
        }
        public static bool IsThereAnyAppointmentsForThisLDLAppID(int LDLAppID, int TestTypeID)
        {
            return clsAppointmentsDataLayer.IsThereAreAnyAppointmentsForThisLDLAppID(LDLAppID, TestTypeID);
        }
        public static DataTable GetAllStreetAppointments(int LDLAppID)
        {
            return clsAppointmentsDataLayer.GetAllStreetTestAppointments(LDLAppID);
        }


    }
}
