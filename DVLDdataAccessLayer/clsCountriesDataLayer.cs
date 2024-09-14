using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLDdataAccessLayer
{
    public class clsCountriesDataLayer
    {
        public static DataTable GetAllCountries()
        {
            DataTable table = new DataTable();
            string Querey = @"Select * from Countries;";
            SqlConnection Connection=new SqlConnection( Settings.ConnectionString );
            SqlCommand Command = new SqlCommand( Querey, Connection );
            try
            {
                Connection.Open();
                SqlDataReader Reader = Command.ExecuteReader();
                if (Reader.HasRows)
                {
                    table.Load(Reader);
                    
                }
                Reader.Close();
            }catch (Exception ex)
            {

            }
            finally
            {
                Connection.Close();

            }
            return table;
        }

        public static string GetCountryName(int CountryID)
        {
            string CountryName = "";
            string Qurey = @"select Countries.CountryName from Countries 
                            where Countries.CountryID=@CountryID; ";
            SqlConnection Connection=new SqlConnection( Settings.ConnectionString );
            SqlCommand Command = new SqlCommand( Qurey, Connection );
            Command.Parameters.AddWithValue("@CountryID", CountryID);
            try
            {
                Connection.Open();
                Object Result = Command.ExecuteScalar();
                if (Result != null)
                {
                    CountryName = Result.ToString();
                }
                
            }catch (Exception ex)
            {
                CountryName = "";
            }
            finally
            {
                Connection.Close();
            }
            return CountryName;
        }
    }
}
