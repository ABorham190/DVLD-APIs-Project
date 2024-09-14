using DVLDdataAccessLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DVDLBussinessLayer
{
    public class clsTestTypes
    {
        public int TestID {  get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public Decimal Fees {  get; set; }

        public clsTestTypes()
        {
            this.Title = "";
            this.Description = "";
            this.TestID = 0;
            this.Fees = 0;
        }
        private clsTestTypes(int id,string title,string describtion,Decimal fees)
        {
            this.TestID = id;
            this.Title = title;
            this.Description = describtion;
            this.Fees = fees;

        }
        public static DataTable GetTestsList()
        {
            return clsTestTypesDataLayer.GetExaminationsList();
        }

        public static clsTestTypes FindTestByID(int TestID)
        {
            string title = "", Describtion = "";
            Decimal Fees = 0;
            if(clsTestTypesDataLayer.FindTestByID(TestID,ref title,ref Describtion,ref Fees))
            {
                return new clsTestTypes(TestID, title, Describtion, Fees);
            }
            else
            {
                return null;
            }
        }
        public bool UpdateTestDetails()
        {
            return clsTestTypesDataLayer.UpdateTestDetails(this.TestID,
                this.Title,this.Description,this.Fees);
        }

        public static int GetTrials(int DLAppID)
        {
            return clsTestTypesDataLayer.GetNumberOfVisionTestFails(DLAppID) ;
        }
        public static int GetPassedTests(int DLAppID)
        {
            return clsTestTypesDataLayer.GetPassedTests(DLAppID) ;
        }
        public static Decimal GetTestFees(int TestTypeID)
        {
            return clsTestTypesDataLayer.GetTestFees(TestTypeID);
        }

    }
}
