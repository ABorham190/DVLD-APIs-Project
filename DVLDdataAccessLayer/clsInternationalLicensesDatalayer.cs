using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DVLDdataAccessLayer
{
    public class clsInternationalLicensesDatalayer
    {
        public static bool FindInternationalLicenseUsingLocalLID(
            ref int licenseID, ref int applicationID,
            ref int driverID, int localLicenseID, 
            ref DateTime issueDate,
            ref DateTime expirationDate, ref bool isActive, 
            ref int createdByUserID)
        {
            bool IsFound = false;
            string Querey = @"select * from InternationalLicenses 
                            where
                            InternationalLicenses.IssuedUsingLocalLicenseID=
                            @LocalLicenseID;";
            SqlConnection Connection = new SqlConnection(Settings.ConnectionString);
            SqlCommand Command = new SqlCommand( Querey, Connection);
            Command.Parameters.AddWithValue("@LocalLicenseID", localLicenseID);

            try
            {
                Connection.Open();
                SqlDataReader Reader = Command.ExecuteReader();
                if (Reader.Read())
                {
                    IsFound = true;
                    licenseID = (int)Reader["InternationalLicenseID"];
                    applicationID = (int)Reader["ApplicationID"];
                    driverID = (int)Reader["DriverID"];
                    localLicenseID = (int)Reader["IssuedUsingLocalLicenseID"];
                    issueDate = (DateTime)Reader["IssueDate"];
                    expirationDate = (DateTime)Reader["ExpirationDate"];
                    isActive = (bool)Reader["IsActive"];
                    createdByUserID = (int)Reader["CreatedByUserID"];

                }
                Reader.Close();
            }catch (Exception ex)
            {
                IsFound = false;
            }
            finally
            {
                Connection.Close();
            }
            return IsFound;

        }

        public static int AddNewInternationalLicense(int ApplicationID,
            int DriverID,int LocalLicenseID,DateTime IssueDate,
            DateTime ExpirationDate,bool IsActive,int CreatedByUserID)
        {
            int InsertedID = 0;
            string Querey = @"INSERT INTO [dbo].[InternationalLicenses]
           (ApplicationID
           ,DriverID
           ,IssuedUsingLocalLicenseID
           ,IssueDate
           ,ExpirationDate
           ,IsActive
           ,CreatedByUserID)
            VALUES
           (@ApplicationID 
           ,@DriverID
           ,@LocalLicenseID
           ,@IssueDate
           ,@ExpirationDate
           ,@IsActive
           ,@CreatedByUserID);
             select scope_Identity();
              ";
            SqlConnection Connection = new SqlConnection(Settings.ConnectionString);
            SqlCommand Command = new SqlCommand(Querey, Connection);
            Command.Parameters.AddWithValue("@ApplicationID", ApplicationID);
            Command.Parameters.AddWithValue("@DriverID", DriverID);
            Command.Parameters.AddWithValue("@LocalLicenseID", LocalLicenseID);
            Command.Parameters.AddWithValue("@IssueDate", IssueDate);
            Command.Parameters.AddWithValue("@ExpirationDate", ExpirationDate);
            Command.Parameters.AddWithValue("@IsActive", IsActive);
            Command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);

            try
            {
                Connection.Open();
                Object Result = Command.ExecuteScalar();
                if(Result!=null&&int.TryParse(Result.ToString(),out int ID))
                {
                    InsertedID = ID;
                }
            }catch (Exception ex)
            {
                InsertedID=0;
            }finally { Connection.Close(); }


            return InsertedID;

        }

        public static bool GetDetailedInternationalLicenseInfoByILID(
             int licenseID, ref int applicationID,
            ref int driverID, ref int localLicenseID,
            ref DateTime issueDate,
            ref DateTime expirationDate, ref bool isActive,
            ref int createdByUserID,ref string Name,ref string NationalNo,
            ref byte Gendor,ref DateTime DateOfBirth,ref string ImagePath)
        {
            bool IsFound=false;

            string Querey = @"select People.FirstName+' '+People.SecondName+' '
                           +People.ThirdName+' '+
                           People.LastName as Name ,People.NationalNo,People.Gendor
                           , InternationalLicenses.ApplicationID, People.DateOfBirth,
                            People.ImagePath
                           ,InternationalLicenses.DriverID
                           ,InternationalLicenses.IssueDate,
                           InternationalLicenses.IssuedUsingLocalLicenseID,InternationalLicenses.ExpirationDate,
                           InternationalLicenses.IsActive,InternationalLicenses.CreatedByUserID 
                           from InternationalLicenses
                           inner join Applications on 
                           InternationalLicenses.ApplicationID=Applications.ApplicationID
                           inner join People on Applications.ApplicantPersonID=People.PersonID
                           where InternationalLicenses.InternationalLicenseID=@InternationalLicenseId;";

            SqlConnection Connection = new SqlConnection(Settings.ConnectionString);
            SqlCommand Command=new SqlCommand(Querey,Connection);
            Command.Parameters.AddWithValue("@InternationalLicenseId", licenseID);

            try
            {
                Connection.Open();
                SqlDataReader Reader = Command.ExecuteReader();
                if (Reader.Read())
                {
                    IsFound = true;
                    
                    applicationID = (int)Reader["ApplicationID"];
                    driverID = (int)Reader["DriverID"];
                    localLicenseID = (int)Reader["IssuedUsingLocalLicenseID"];
                    issueDate = (DateTime)Reader["IssueDate"];
                    expirationDate = (DateTime)Reader["ExpirationDate"];
                    isActive = (bool)Reader["IsActive"];
                    createdByUserID = (int)Reader["CreatedByUserID"];
                    Name = (string)Reader["Name"];
                    NationalNo = (string)Reader["NationalNo"];
                    Gendor = (byte)Reader["Gendor"];
                    DateOfBirth = (DateTime)Reader["DateOfBirth"];

                    if (Reader["ImagePath"] == DBNull.Value)
                    {
                        ImagePath = "";
                    }
                    else
                    {
                        ImagePath = (string)Reader["ImagePath"];
                    }

                }
                Reader.Close();
            }catch (Exception ex)
            {
                IsFound=false;
            }
            finally
            {
                Connection.Close();
            }
            return IsFound;
                           
        }

        public static DataTable GetInternationalLicenseHistoryForPersonByPersonID
            (int PersonID)
        {
            DataTable dt = new DataTable();
            string Querey = @"select InternationalLicenseID as IntLID,ApplicationID,
                            IssuedUsingLocalLicenseID as LID,IssueDate,ExpirationDate,
                            IsActive
                            from InternationalLicenses inner join Drivers
                            on InternationalLicenses.DriverID=Drivers.DriverID
                            where Drivers.PersonID=@PersonID;";
            SqlConnection Connection = new SqlConnection(Settings.ConnectionString);
            SqlCommand Command=new SqlCommand(Querey,Connection);
            Command.Parameters.AddWithValue("@PersonID", PersonID);

            try
            {
                Connection.Open();
                SqlDataReader Reader = Command.ExecuteReader();
                if (Reader.HasRows)
                {
                    dt.Load(Reader);
                }
                Reader.Close();
            }catch(Exception ex) 
            {
                MessageBox.Show(ex.Message);     
            }
            finally
            {
                Connection.Close();
            }
            return dt;
        }

        
    }
}
