using DVLDdataAccessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Runtime.CompilerServices;
using System.Data.SqlClient;
using System.Security.Policy;
using System.Threading;

namespace DVDLBussinessLayer
{
    public class clsOrders
    {
        public int ApplicationID { get; set; }
        public int ApplicantID { get; set; }
        public DateTime ApplicationDate { get; set; }
        public int ApplicationTypeID { get; set; }//Remember to delete this line

        public byte ApplicationStatus { get; set; }
        public int PassedTests { get; set; }  //Remember to delete this line
        public DateTime LastStatusDate { get; set; }
        public Decimal PaidFees { get; set; }

        public int LicesneTypeID { get; set; }  //Remember to delete this line
        //public byte ApplicationTypeID {  get; set; }
        public int CreatedByUserID { get; set; }
        public string AppStatus { get; set; }




        clsOrders(int applicationID, int personID, string applicationStatus, DateTime applicationDate,
           int OrderNameID)
        {
            ApplicationID = applicationID;
            ApplicantID = personID;
            AppStatus = applicationStatus;

            ApplicationDate = applicationDate;
            ApplicationTypeID = OrderNameID;

        }
        public clsOrders()
        {
            ApplicationID = 0;
            ApplicantID = 0;
            ApplicationStatus = 0;
            PassedTests = 0;
            ApplicationDate = DateTime.MinValue;
            ApplicationTypeID = 0;

        }

        public static DataTable GetAllOrdersInSystem()
        {
            return clsOrdersDataLayer.GetAllOrders();
        }

        public bool AddNewApplication()
        {
            this.ApplicationID = clsOrdersDataLayer.AddNewOrder(this.ApplicantID, this.ApplicationDate,
                this.ApplicationTypeID, this.ApplicationStatus, this.LastStatusDate,
                this.PaidFees, this.CreatedByUserID);

            return this.ApplicationID != -1;
        }

        public bool Save()
        {
            if (AddNewApplication())
            {
                return true;
            }
            return false;
        }

        public static bool IsApplicationExist(int PersonID, int LicenseTypeID, ref int ApplicationID)
        {
            return clsOrdersDataLayer.IsApplicationExists(PersonID, LicenseTypeID, ref ApplicationID);
        }

        public static bool CancelApplication(int OrderID)
        {
            return clsOrdersDataLayer.CancelOrder(OrderID);
        }

        public static clsOrders FindOrder(int OrderID)
        {
            int PersonID = 0, PassedTest = 0;
            byte OrderCaseID = 0;
            int OrderNameID = 0;
            string AppStatus = "";
            DateTime OrderDate = DateTime.MinValue;
            if (clsOrdersDataLayer.FindOrder(OrderID, ref PersonID, ref AppStatus,
                ref OrderDate, ref OrderNameID))
            {
                return new clsOrders(OrderID, PersonID, AppStatus,
                     OrderDate, OrderNameID);
            }
            else
            {
                return null;
            }
        }

        public static bool GetStatusName(int OrderCaseID, ref string Status)
        {
            return clsOrdersDataLayer.GetStatusName(OrderCaseID, ref Status);
        }

        public static bool GetServiceNameAndApplicationFees(int ServiceNameID, ref Decimal Fees,
            ref string ServiceName)
        {
            return clsOrdersDataLayer.GetServiceNameAndFees(ServiceNameID,
                ref ServiceName, ref Fees);
        }

        public static bool EditPasseTests(int ApplicationID)
        {
            return clsOrdersDataLayer.EditPassedTests(ApplicationID);
        }

        public static bool GetAppFees(string ServiceName, ref float Fees)
        {
            return clsOrdersDataLayer.GetApplicationFeesUsingName(ServiceName, ref Fees);
        }

        public static bool CompleteOrder(int OrderID)
        {
            return clsOrdersDataLayer.CompleteOrder(OrderID);
        }
        public static DataTable GetAllInternationalApplications()
        {
            return clsOrdersDataLayer.GetAllApplicationForInternationalLicenses();
        }
    }
}
