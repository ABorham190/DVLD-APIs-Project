using DVDLBussinessLayer;

namespace dvld_api.models
{
    public class clsHandleLDLApp
    {
        clsOrders Application {  get; set; }
        LDLApp ldlApp { get; set; }

        public clsHandleLDLApp(int personID, int licenseTypeID)
        {  
            this.Application = new clsOrders();
            this.ldlApp=new LDLApp();

            //fill application object
            this.Application.ApplicantID = personID;
            this.Application.ApplicationDate = DateTime.Now;
            this.Application.ApplicationTypeID = 1;
            this.Application.ApplicationStatus = 1;
            this.Application.LastStatusDate = DateTime.Now;
            decimal appfees = 0;
            if (clsOrders.GetAppFees("New Local Driving License Service", ref appfees))
            {
                this.Application.PaidFees = appfees;
            }
            this.Application.CreatedByUserID = 1;

            //fill ldlApp Object
            this.ldlApp.LicenseTypeID = licenseTypeID;
        }

        public bool Save()
        {
            if (this.Application.Save())
            {
                this.ldlApp.AppID = this.Application.ApplicationID;
                if (this.ldlApp.Save())
                {
                    return true;
                }
            }
            else
            {
                return false;
            }

            return false;

        }
    }


    
}
