using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLDdataAccessLayer
{
    public class clsDriversDataLayer
    {
        public static int AddNewDriver(int PersonID,int CreatedByUserID,
            DateTime CreateDate)
        {
            int InsertedID = 0;
            string Querey = @"Insert into Drivers (PersonID,CreatedByUserID,
                           CreatedDate) values (@PersonID,@CreatedByUserID,@CreateDate);
                           select scope_Identity();";

            SqlConnection Connection=new SqlConnection(Settings.ConnectionString);
            SqlCommand Command=new SqlCommand(Querey, Connection);

            Command.Parameters.AddWithValue("@PersonID", PersonID);
            Command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);
            Command.Parameters.AddWithValue("@CreateDate", CreateDate);

            try
            {
                Connection.Open();
                Object Result = Command.ExecuteScalar();
                if(Result!=null&&int.TryParse(Result.ToString(), out int ID))
                {
                    InsertedID = ID;
                }
            }catch (Exception ex)
            {
                InsertedID = 0;
            }
            finally
            {
                Connection.Close();
            }
            return InsertedID;
        }

        public static bool IsThisPersonADriver(int PersonID,ref int DriverID)
        {
            bool IsFound=false;
            string Querey = @"select Found=1,Drivers.DriverID from Drivers 
                            where Drivers.PersonID=@PersonID;";
            SqlConnection Connection=new SqlConnection(Settings.ConnectionString);
            SqlCommand Command=new SqlCommand( Querey, Connection);
            Command.Parameters.AddWithValue("@PersonID", PersonID);
            try
            {
                Connection.Open();
                SqlDataReader Reader = Command.ExecuteReader();

                if(Reader.Read())
                {
                    IsFound = true;
                    DriverID = (int)Reader["DriverID"];
                }

                Reader.Close();

            }catch (Exception ex)
            {
                IsFound = false;
            }finally { Connection.Close(); }
            return IsFound;
        }

        public static DataTable GetManageDriversDetails()
        {
            DataTable Table = new DataTable();
            string Querey = @"select Table1.DriverID,Table1.PersonID,Table1.NationalNo,
                           Table1.Name,Table1.CreatedDate,
                            count(Licenses.ApplicationID) As ActiveLicenses from
                           (select Drivers.DriverID,Drivers.PersonID
                           ,People.NationalNo,People.FirstName+' '+
                           People.SecondName+' '+People.ThirdName+' '
                           +People.LastName as Name,Drivers.CreatedDate
                           from Drivers inner join People
                           on Drivers.PersonID=People.PersonID)Table1
                           inner join 
                           Licenses on Licenses.DriverID=Table1.DriverID 
                           and Licenses.IsActive=1
                           group by Table1.DriverID , 
                           Table1.PersonID,Table1.NationalNo,Table1.Name,
                           Table1.CreatedDate;
                           ";

            SqlConnection Connection=new SqlConnection(Settings.ConnectionString);
            SqlCommand Command=new SqlCommand(Querey, Connection);
            try
            {
                Connection.Open();
                SqlDataReader Reader = Command.ExecuteReader();
                if (Reader.HasRows)
                {
                    Table.Load(Reader);
                }
                Reader.Close();
            }catch(Exception ex)
            {

            }
            finally
            {
                Connection.Close();
            }
            return Table;
        }
    }
}
