using DVLDdataAccessLayer;
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




        private bool _AddNewDetain()
        {
            
            this.DetainID = clsDetainDataLayer.AddNewDetain(this.LicenseID,
                this.DetainDate, this.FineFees, this.CreatedByUserID,
                this.IsRelease);
            return this.DetainID != 0;
        }

        private bool _UpdateDetain()
        {
            return clsDetainDataLayer.UpdateDetain(this.DetainID, this.IsRelease,
                this.ReleaseDate, this.ReleasedByUserId, this.ReleasedAppID);
        }

        public bool Save()
        {
            switch (_Mode)
            {
                case enMode.AddNew:
                    if (_AddNewDetain())
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

        public static DataTable GetAllDetainedLicenses()
        {
            return clsDetainDataLayer.GetAllDetainedLicenses();
        }
        

    }
}
