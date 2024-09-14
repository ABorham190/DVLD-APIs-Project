using DVLDdataAccessLayer;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace DVDLBussinessLayer
{
    public class clsInternationalLicense
    {
        public int LicenseID { get; set; }
        public int ApplicationID { get; set; }
        public int DriverID { get; set; }
        public int LocalLicenseID { get; set; }
        public DateTime IssueDate {  get; set; }
        public DateTime ExpirationDate { get; set; }
        public bool IsActive {  get; set; }
        public int CreatedByUserID {  get; set; }

        public string Name { get; set; }
        public string NationalNo { get; set; }
        public byte Gender { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string ImagePath {  get; set; }

        enum enMode { Update=1,AddNew=2 }
        enMode _Mode;
        clsInternationalLicense(int licenseID, int applicationID, 
            int driverID, int localLicenseID, DateTime issueDate, 
            DateTime expirationDate, bool isActive, int createdByUserID)
        {
            LicenseID = licenseID;
            ApplicationID = applicationID;
            DriverID = driverID;
            LocalLicenseID = localLicenseID;
            IssueDate = issueDate;
            ExpirationDate = expirationDate;
            IsActive = isActive;
            CreatedByUserID = createdByUserID;
            _Mode = enMode.Update;

        }
        clsInternationalLicense(int licenseID, int applicationID,
           int driverID, int localLicenseID, DateTime issueDate,
           DateTime expirationDate, bool isActive, int createdByUserID,string
            name,string nationalnumber,byte gendor,DateTime DateOfBirth,string imagepath)
        {
            this.LicenseID = licenseID;
            this.ApplicationID = applicationID;
            this.DriverID = driverID;
            this.LocalLicenseID = localLicenseID;
            this.IssueDate = issueDate;
            this.ExpirationDate = expirationDate;
            this.IsActive = isActive;
            this.CreatedByUserID = createdByUserID;
            this.Name= name;
            this.NationalNo = nationalnumber;
            this.Gender = gendor;
            this.DateOfBirth = DateOfBirth;
            this.ImagePath = imagepath;
            _Mode = enMode.Update;

        }
        public clsInternationalLicense()
        {

            LicenseID = 0;
            ApplicationID = 0;
            DriverID = 0;
            LocalLicenseID = 0;
            IssueDate = DateTime.MinValue;
            ExpirationDate = DateTime.MinValue;
            IsActive = false;
            CreatedByUserID = 0;

            _Mode = enMode.AddNew;

        }

        public static clsInternationalLicense FindInterNationalLicense(int LocalLicenseID)
        {
            int licenseID = 0, applicationID = 0, driverID = 0, createdByUserID = 0;
            DateTime issuedate= DateTime.MinValue, expirationdate= DateTime.MinValue;
            bool isActive = false;
            if(clsInternationalLicensesDatalayer.FindInternationalLicenseUsingLocalLID
                (ref licenseID,ref applicationID,ref driverID,LocalLicenseID,
                ref issuedate,ref expirationdate,ref isActive,ref createdByUserID))
            {
                return new clsInternationalLicense(licenseID, applicationID, driverID,
                    LocalLicenseID, issuedate, expirationdate, isActive, createdByUserID);
            }
            else
            {
                return null;
            }
        }

        public static clsInternationalLicense FindDetailedInternationalLicenseInfoByILID(int ILID)
        {
            int  applicationID = 0, driverID = 0, createdByUserID = 0;
            DateTime issuedate = DateTime.MinValue, expirationdate = DateTime.MinValue;
            bool isActive = false;
            string Name = "", NationalNumber = "";
            DateTime DateOfBirht= DateTime.MinValue;
            byte Gender = 0;
            int localLicenseID = 0;
            string ImagePath = "";
            if(clsInternationalLicensesDatalayer.GetDetailedInternationalLicenseInfoByILID
                (ILID,ref applicationID,ref driverID,ref localLicenseID,ref issuedate,
                ref expirationdate,ref isActive,ref createdByUserID,
                ref Name,ref NationalNumber,ref Gender,ref DateOfBirht,ref ImagePath))
            {

                return new clsInternationalLicense(ILID, applicationID, driverID,
                    localLicenseID, issuedate, expirationdate, isActive, createdByUserID,
                    Name,NationalNumber,Gender,DateOfBirht,ImagePath);
            }
            else
            {
                return null;
            }
        }

        private bool _AddNewInternationalLicense()
        {
            this.LicenseID = clsInternationalLicensesDatalayer.
                AddNewInternationalLicense(this.ApplicationID, this.DriverID,
                this.LocalLicenseID, this.IssueDate, this.ExpirationDate,
                this.IsActive, this.CreatedByUserID);
            return this.LicenseID != 0;
        }

        public bool Save()
        {
            switch (_Mode)
            {
                case enMode.AddNew:
                  if(_AddNewInternationalLicense())
                    {
                        return true;
                    }
                    break;
            }
            return false;
        }

        public static DataTable GetPersonInternationalLicenseHistory(int PersonID)
        {
            return clsInternationalLicensesDatalayer.GetInternationalLicenseHistoryForPersonByPersonID(PersonID);
        }
    }

}
