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

        public clsPerson  Person { get; set; }
        public int CreatedByUserID { get; set; }
        public string AppStatus { get; set; }





        public clsOrders(int applicationID, int personID, byte applicationStatus, DateTime applicationDate,
           int OrderNameID)
        {
            ApplicationID = applicationID;
            ApplicantID = personID;
            ApplicationStatus = applicationStatus;

            ApplicationDate = applicationDate;
            ApplicationTypeID = OrderNameID;

            Person=clsPerson.FindPerson(ApplicantID);
            

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

        public bool AddNewApplication()
        {
            this.ApplicationID = clsOrdersDataLayer.AddNewApplication(this.ApplicantID, this.ApplicationDate,
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

        public static bool IsThisPersonIDHasAnActiveApplicationToThisLicenseTypeID(int PersonID, int LicenseTypeID, ref int ApplicationID)
        {
            return clsOrdersDataLayer.IsThisPersonIDHasAnActiveApplicationForThisLicenseTypeID(PersonID, LicenseTypeID, ref ApplicationID);
        }

       
        public static async Task<bool> UpdateApplicationStatus(int ApplicationID,clsOrdersDataLayer.enWhatToDo whattodo)
        {
            return await clsOrdersDataLayer.UpdateApplicationStatus(ApplicationID, whattodo);
        }

        public static clsOrders FindApplicationByID(int OrderID)
        {
            int PersonID = 0, PassedTest = 0;
            byte OrderCaseID = 0;
            int OrderNameID = 0;
            string AppStatus = "";
            byte ApplicationStatusID = 0;
            DateTime OrderDate = DateTime.MinValue;
            if (clsOrdersDataLayer.FindApplicationByID(OrderID, ref PersonID, 
                ref OrderDate, ref OrderNameID,ref ApplicationStatusID))
            {
                return new clsOrders(OrderID, PersonID, ApplicationStatusID,
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

        public static bool GetAppFees(string ServiceName, ref decimal Fees)
        {
            return clsOrdersDataLayer.GetApplicationFeesUsingName(ServiceName, ref Fees);
        }

      
        public static DataTable GetAllInternationalApplications()
        {
            return clsOrdersDataLayer.GetAllApplicationForInternationalLicenses();
        }
    }
}
