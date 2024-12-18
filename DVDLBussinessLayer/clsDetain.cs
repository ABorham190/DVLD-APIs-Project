﻿using DVLDdataAccessLayer;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVDLBussinessLayer
{
    public class clsDetain
    {
        public int DetainID {  get; set; }
        public int LicenseID {  get; set; }
        public DateTime DetainDate { get; set; }

        public Decimal FineFees {  get; set; }
        public int CreatedByUserID {  get; set; }
        public bool IsRelease {  get; set; }

        public DateTime ReleaseDate { get; set; } 
        public int ReleasedByUserId {  get; set; }
        public int ReleasedAppID {  get; set; }


        enum enMode { AddNew,Update}
        enMode _Mode;
        public clsDetain()
        {
            DetainID = 0;

            LicenseID = 0;
            DetainDate = DateTime.MinValue;
            FineFees = 0;
            CreatedByUserID = 0;
            IsRelease = false;

            _Mode=enMode.AddNew;
        }

        clsDetain(int detainID,DateTime Detaindate,Decimal FineFees,
            int Createdbyuserid)
        {
            this.DetainID = detainID;
            this.DetainDate = Detaindate;
            this.FineFees = FineFees;
            this.CreatedByUserID = Createdbyuserid;

            _Mode = enMode.Update;
        }

        public static clsDetain FindDetain(int LicenseID)
        {
            int DetainID = 0, CreatedByUserID = 0;
            DateTime detainDate = DateTime.MinValue;
            Decimal FineFees = 0;

            if(clsDetainDataLayer.FindDetainByLicenseID(LicenseID,
                ref DetainID,ref detainDate,ref FineFees,ref CreatedByUserID))
            {
                return new clsDetain(DetainID,detainDate,FineFees,CreatedByUserID);
            }
            else
            {
                return null;
            }
        }

        
        private async Task< bool> _AddNewDetain()
        {
            clsLicenses licenses = clsLicenses.GetLicenseDetailsByLLicenseID(this.LicenseID);
            if (licenses.detain != null)
            {
                return false;
            }
            
            this.DetainID =await clsDetainDataLayer.AddNewDetain(this.LicenseID,
                this.DetainDate, this.FineFees, this.CreatedByUserID,
                this.IsRelease);
            return this.DetainID != 0;
        }

        private bool _UpdateDetain()
        {
            return clsDetainDataLayer.UpdateDetain(this.DetainID, this.IsRelease,
                this.ReleaseDate, this.ReleasedByUserId, this.ReleasedAppID);
        }

        public async Task<bool> Save()
        {
            switch (_Mode)
            {
                case enMode.AddNew:
                    if (await _AddNewDetain())
                    {
                        _Mode = enMode.Update;
                        return true;
                    }
                    break;

                case enMode.Update:
                    if (_UpdateDetain())
                    {
                        return true;
                    }
                    break;
            }

            return false;
        }

        public static async Task<List<clsDetainDataLayer.GetDetianedLicenseDTO>> GetAllDetainedLicenses()
        {
            return await clsDetainDataLayer.GetAllDetainedLicenses();
        }
        
        public static async Task<bool>ReleaseDetainedLicense(int LicenseID,int CreatedByUserID)
        {
            Log.Information("Start execution of ReleaseDetainedLicense method in clsDetain");
            
            clsLicenses DetainedLicense=clsLicenses.GetLicenseDetailsByLLicenseID(LicenseID);
            if (DetainedLicense == null)
            {
                Log.Error($"License with LicenseID : {LicenseID} is not found");
                return false;
            }

            if (DetainedLicense.detain == null)
            {
                Log.Error($"License with LicenseID : {DetainedLicense.LicenseID} is not detained");
                return false;
            }

            clsOrders ReleaseApp=new clsOrders 
            {
                ApplicantID=DetainedLicense.Person.ID,
                ApplicationTypeID=5,
                ApplicationDate=DateTime.Now,
                PaidFees=15,
                CreatedByUserID=1,
                LastStatusDate=DateTime.Now,
                
            };

            if (!ReleaseApp.Save())
            {
                Log.Error("Release Application NOT added successfully");
                return false;
            }

            return await clsDetainDataLayer.ReleaseDetainedLicense(LicenseID, ReleaseApp.ApplicationID, CreatedByUserID) > 0;
        }
    }
}
