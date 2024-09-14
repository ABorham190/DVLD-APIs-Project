using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace DVLDdataAccessLayer
{
    public  class clsServicesDataLayer
    {
        public static DataTable GetServicesList()
        {
            DataTable dt= new DataTable();
            string Querey = @"select * from ApplicationTypes;";
            SqlConnection Connection = new SqlConnection(Settings.ConnectionString);
            SqlCommand Command = new SqlCommand(Querey, Connection);
            try
            {
                Connection.Open();
                SqlDataReader Reader = Command.ExecuteReader();
                if (Reader.HasRows)
                {
                    dt.Load(Reader);
                }
                Reader.Close();
            }catch (Exception ex)
            {
                
            }finally 
            {
                Connection.Close(); 
            }
            return dt;
        }

        public static bool FindServiceDetails(int ID,ref string Name,
            ref float Fees)
        {
            bool IsFound = false;
            string Querey = @"select * from NameOfService where ServiceNameID=@ID;";
            SqlConnection Connection=new SqlConnection(Settings.ConnectionString);
            SqlCommand Command = new SqlCommand( Querey, Connection);
            Command.Parameters.AddWithValue("@ID", ID);

            try
            {
                Connection.Open();
                SqlDataReader Reader = Command.ExecuteReader();
                if (Reader.Read())
                {
                    IsFound = true;
                    Name = (string)Reader["ServiceName"];
                    float.TryParse(Reader["ServiceFees"].ToString(), out float fees);
                    Fees = fees;
                }
                Reader.Close();
            }catch(Exception ex)
            {
                IsFound = false;
            }finally { Connection.Close();}

            return IsFound;
        }

        public static bool UpdateServiceDetails(int ID,string Name,
            float Fees)
        {
            int NumberOfAffectedRows = 0;
            string Querey = @"update NameOfService  set ServiceName=
                            @Name,ServiceFees=@Fees
                            where ServiceNameID=@ID;";
            SqlConnection connection = new SqlConnection(Settings.ConnectionString);
            SqlCommand Command = new SqlCommand(Querey, connection);
            Command.Parameters.AddWithValue("@Name", Name);
            Command.Parameters.AddWithValue("@Fees", Fees);
            Command.Parameters.AddWithValue("@ID", ID);


            try
            {
                connection.Open();
                NumberOfAffectedRows = Command.ExecuteNonQuery();

            }catch (Exception ex)
            {
                NumberOfAffectedRows = 0;
            }
            finally { connection.Close(); }

            return NumberOfAffectedRows > 0;

        }
    }
}
