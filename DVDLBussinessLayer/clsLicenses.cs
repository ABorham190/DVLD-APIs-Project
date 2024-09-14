using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using DVLDdataAccessLayer;
using System.Runtime.CompilerServices;

namespace DVDLBussinessLayer
{
    public  class clsLicenses
    {
        public int LicenseID;
        public int ApplicationID;
        public int DriverID;
        public int LicenseClassID;
        public DateTime IssueDate;
        public DateTime ExpirationDate;
        public string Notes;
        public Decimal PaidFees;
        public bool IsActive;
        public byte IssueReason;
        public int CreatedByUserID;
        public string ImagePath;
        public string ClassName;
        public string NationalNo;
        public bool IsDetained;
        public DateTime DateOfBirth;
        public string Name;
        public byte Gender;
        enum enMode { UpdateMode = 1, AddNewMode = 2 }
        enMode _Mode;
       public clsLicenses()
        {
            LicenseID=0;
            ApplicationID=0;
            DriverID = 0;
            LicenseClassID = 0;
                IssueDate=DateTime.Now;
            ExpirationDate=DateTime.Now;
            Notes="";
            PaidFees = 0;
            IsActive=false;
            IssueReason = 0;
            CreatedByUserID = 0;
            ImagePath = "";
            ClassName = "";
            NationalNo = "";
            IsDetained=false;
            Gender= 0;
            

            _Mode = enMode.AddNewMode;
        }
        private clsLicenses(int licenseID, int applicationID,string name,
            DateTime dateofbirth,byte gender,
            int driverID,  DateTime issueDate,
            DateTime expirationDate, string notes, 
            bool isActive, byte issueReason,  
            string imagePath, string className, string nationalNo
            )
        {
            LicenseID = licenseID;
            ApplicationID = applicationID;
            DriverID = driverID;
            Name= name;
            IssueDate = issueDate;
            ExpirationDate = expirationDate;
            Notes = notes;
            DateOfBirth = dateofbirth;
            Gender = gender;
            
            IsActive = isActive;
            IssueReason = issueReason;
            
            ImagePath = imagePath;
            ClassName = className;
            NationalNo = nationalNo;
            
            _Mode = enMode.UpdateMode;
        }

        public static DataTable GetLicenseClasses()
        {
            return clsLicensesDataLayer.GetLicenseClasses();
        }
        public static bool GetLicenseClass(int LicenseClassID,ref string LicenseClass)
        {
            return clsLicensesDataLayer.GetLicenseClass(LicenseClassID,ref LicenseClass);
        }
        private bool _AddNewDrivingLicenseFirstTime()
        {
            this.LicenseID = clsLicensesDataLayer.IssueLDLFirstTime(
            this.ApplicationID, this.DriverID, this.LicenseClassID,
            this.IssueDate, this.ExpirationDate, this.Notes,
            this.PaidFees, this.IsActive, this.IssueReason,
            this.CreatedByUserID);

            return this.LicenseID != 0;

        }
        public static clsLicenses GetLicenseDetailsByLDLAppID(int LDLAppID)
        {
            int LicenseID=0;
            int ApplicationID=0;
            int DriverID = 0;
            int LicenseClassID = 0;
            DateTime IssueDate=DateTime.Now;
            DateTime ExpirationDate=DateTime.Now;
            string Notes="";
            Decimal PaidFees = 0;
            bool IsActive=false;
            byte IssueReason = 0;
            
            string ImagePath="";
            string ClassName="";
            string NationalNo = "";
            string ApplicantName = "";
            DateTime DateOfBirth=DateTime.Now;
            byte Gender = 0;
            if(clsLicensesDataLayer.GetLicenseDetailsUsingLDLAppID(LDLAppID,ref ClassName
                ,ref ApplicantName,ref LicenseID,ref NationalNo,
                ref ImagePath,ref IssueDate,ref IssueReason,ref Notes,
                ref IsActive,ref  DateOfBirth,ref Gender,ref DriverID,
                ref ExpirationDate))
            {
                return new clsLicenses(LicenseID, ApplicationID,ApplicantName,DateOfBirth,
                    Gender,DriverID, IssueDate, ExpirationDate, Notes, IsActive,
                    IssueReason, ImagePath, ClassName, NationalNo
                    );
            }
            else
            {
                return null;
            }
        }

        public bool Save()
        {
            switch (_Mode)
            {
                case enMode.AddNewMode:
                    if (_AddNewDrivingLicenseFirstTime())
                    {
                        _Mode = enMode.UpdateMode;
                        return true;
                    }
                    break;

            }
            return false;
        }
        public static bool IsThisLicenseStillDetained(int LicenseID)
        {
            return clsLicensesDataLayer.IsLicenseStillDetained(LicenseID);
        }

        public static bool IsThisPersonHasLicenseFromThisType(int PersonID,
            int LicenseClassID,ref int LicenseID)
        {
            return clsLicensesDataLayer.IsThisPersonHasLicenseFromTheSameClass(PersonID, LicenseClassID, ref LicenseID);
        }

        public static DataTable GetPersonLocalLicenseHistory(int PersonID)
        {
            return clsLicensesDataLayer.GetLocalLicensesHistoryForAPerson(PersonID);
        }

        public static clsLicenses GetLicenseDetailsByLLicenseID(int LicenseID)
        {
            
            int ApplicationID = 0;
            int DriverID = 0;
            int LicenseClassID = 0;
            DateTime IssueDate = DateTime.Now;
            DateTime ExpirationDate = DateTime.Now;
            string Notes = "";
            Decimal PaidFees = 0;
            bool IsActive = false;
            byte IssueReason = 0;

            string ImagePath = "";
            string ClassName = "";
            string NationalNo = "";
            string ApplicantName = "";
            DateTime DateOfBirth = DateTime.Now;
            byte Gender = 0;
            if (clsLicensesDataLayer.GetLicenseDetailsUsingLicenseID( ref ClassName
                , ref ApplicantName,  LicenseID, ref NationalNo,
                ref ImagePath, ref IssueDate, ref IssueReason, ref Notes,
                ref IsActive, ref DateOfBirth, ref Gender, ref DriverID,
                ref ExpirationDate))
            {
                return new clsLicenses(LicenseID, ApplicationID, ApplicantName, DateOfBirth,
                    Gender, DriverID, IssueDate, ExpirationDate, Notes, IsActive,
                    IssueReason, ImagePath, ClassName, NationalNo
                    );
            }
            else
            {
                return null;
            }
        }

        public static int GetDriverIDByLocalLicenseID(int LocalLicenseID)
        {
            return clsLicensesDataLayer.GetDriverIDByLocalLicenseID(LocalLicenseID);
        }

        public static bool GetLicenseClassFeesByLicenseClassID(int LicenseClassID,ref Decimal Fees)
        {
            return clsLicensesDataLayer.GetLicenseFeesUsingLicenseClassID(
                LicenseClassID, ref Fees);
        }

        public static bool GetLicenseClassFeesByClassName(string ClassName,
            ref Decimal LicenseFees)
        {
            return clsLicensesDataLayer.GetLicenseFeesUsingLicenseClassName(
                ClassName, ref LicenseFees);
        }

        public static int GetLicenseClassIDByclassName(string ClassName)
        {
            return clsLicensesDataLayer.GetLicenseClassIDByClassName(ClassName);
        }

        public static bool DeactivateLocalLicenseByLicenseID(int LicenseID)
        {
            return clsLicensesDataLayer.DeActivateLicenseByID(LicenseID);
        }

    }
}
