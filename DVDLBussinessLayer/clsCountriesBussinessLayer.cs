using DVLDdataAccessLayer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVDLBussinessLayer
{
    public class clsCountriesBussinessLayer
    {
        public static DataTable GetAllCountries()
        {
            return clsCountriesDataLayer.GetAllCountries();
        }
        public static string GetCountryName(int CountryID)
        {
            return clsCountriesDataLayer.GetCountryName(CountryID);
        }
    }
}
