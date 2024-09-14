using DVLDdataAccessLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVDLBussinessLayer
{
    public class clsServices
    {
        public int ID {  get; set; }
        public string Name { get; set; }
        public float Fees {  get; set; }

        public static DataTable GetServicesList()
        {
            return clsServicesDataLayer.GetServicesList();
        }
        public clsServices()
        {
            this.ID = 0;
            this.Name = "";
            this.Fees = 0;
        }
        clsServices(int ID,string Name,float Fees)
        {
            this.ID = ID;
            this.Name = Name;
            this.Fees = Fees;
        }

        public static clsServices FindServiceByID(int ID)
        {
            string Name = "";
            float Fees = 0;
            if(clsServicesDataLayer.FindServiceDetails(ID,ref Name,ref Fees))
            {
                return new clsServices(ID,Name,Fees);
            }
            else
            {
                return null;    
            }
        }

        public bool UpdateServiceDetails()
        {
            if(clsServicesDataLayer.UpdateServiceDetails(this.ID,this.Name,
                this.Fees))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


    }
}
