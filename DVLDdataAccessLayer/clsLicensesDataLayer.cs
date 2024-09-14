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
    public class clsLicensesDataLayer
    {
        public static DataTable GetLicenseClasses()
        {
            DataTable table = new DataTable();
            string Querey = @"select ClassName from LicenseClasses;";
            SqlConnection Connection = new SqlConnection(Settings.ConnectionString);
            SqlCommand Command=new SqlCommand(Querey,Connection);
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

        public static bool GetLicenseClass(int LicenseClassTypeID,ref string LicenseType)
        {
            bool IsFound=false;
            string Querey = @"select ClassName from LicenseClasses
                             where LicenseClassID=@LicenseClassTypeID;";
            SqlConnection Connection =new SqlConnection(Settings.ConnectionString); 
            SqlCommand Command =new SqlCommand( Querey,Connection);
            Command.Parameters.AddWithValue("@LicenseClassTypeID", LicenseClassTypeID);

            try
            {
                Connection.Open();
                SqlDataReader Reader = Command.ExecuteReader();
                if (Reader.Read())
                {
                    IsFound = true;
                    LicenseType = (string)Reader["ClassName"];
                }
                Reader.Close();
            }catch(Exception ex)
            {
                IsFound = false;
            }finally 
            {
                Connection.Close();
            }

            return IsFound;
        }
        public static int IssueLDLFirstTime(int ApplicationID,int DriverID,
            int LicenseClassID,DateTime IssueDate,DateTime ExpirationDate,
            string Notes,Decimal PaidFees,bool IsActive,byte IssueReason,
            int CreatedByUserID)
        {
            int InsertedID = 0;
            string Querey = @"Insert into Licenses (ApplicationID,
                             DriverID,LicenseClass,IssueDate,ExpirationDate,
                             Notes,PaidFees,IsActive,IssueReason,CreatedByUserID) values 
                             (@ApplicationID,@DriverID,@LicenseClass,@IssueDate,@ExpirationDate,
                             @Notes,@PaidFees,@IsActive,@IssueReason,@CreatedByUserID);
                             Select scope_identity();";

            SqlConnection Connection=new SqlConnection(Settings.ConnectionString);  
            SqlCommand Command =new SqlCommand(Querey,Connection);
            Command.Parameters.AddWithValue("@ApplicationID", ApplicationID);
            Command.Parameters.AddWithValue("@DriverID", DriverID);
            Command.Parameters.AddWithValue("@LicenseClass", LicenseClassID);
            Command.Parameters.AddWithValue("@IssueDate", IssueDate);
            Command.Parameters.AddWithValue("@ExpirationDate", ExpirationDate);
            if (Notes != "")
                Command.Parameters.AddWithValue("@Notes", Notes);
            else
                Command.Parameters.AddWithValue("@Notes", DBNull.Value);
            Command.Parameters.AddWithValue("@PaidFees", PaidFees);
            Command.Parameters.AddWithValue("@IsActive", IsActive);
            Command.Parameters.AddWithValue("@IssueReason", IssueReason);
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
            }
            finally
            {
                Connection.Close();
            }

            return InsertedID;
        }

        public static bool GetLicenseDetailsUsingLDLAppID(int LDLAppID,
            ref string LicenseClass,ref string Name,ref int LicenseID,
            ref string NationalNo,ref string ImagePath,ref DateTime IssueDate,ref byte IssueReason,
            ref string Notes,ref bool IsActive,ref DateTime DateOfBirth,
            ref byte Gender,ref int DriverID,ref DateTime ExpirationDate)
        {
            bool IsFound = false;
            string Querey = @"select Licenses.LicenseID,
                            (People.FirstName + ' ' + People.SecondName + ' '
                            + People.ThirdName + ' ' + People.LastName) AS Name,People.Gendor,
                            LicenseClasses.ClassName,People.NationalNo,People.ImagePath,
                            Licenses.IssueDate,Licenses.IssueReason,Licenses.Notes,
                            Licenses.IsActive,People.DateOfBirth,Drivers.DriverID,
                            Licenses.ExpirationDate from Licenses inner join 
                            Applications on Licenses.ApplicationID=Applications.ApplicationID
                            inner join LocalDrivingLicenseApplications on 
                            LocalDrivingLicenseApplications.ApplicationID=
                            Applications.ApplicationID inner join People on
                            Applications.ApplicantPersonID=People.PersonID 
                            inner join LicenseClasses on Licenses.LicenseClass=
                            LicenseClasses.LicenseClassID inner join Drivers on
                            Licenses.DriverID=Drivers.DriverID
                            
                            where LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID=@LDLAppId;
                            
                            ";
            SqlConnection Connection = new SqlConnection(Settings.ConnectionString);
            SqlCommand Command=new SqlCommand(Querey,Connection);
            Command.Parameters.AddWithValue("@LDLAppId", LDLAppID);
            try
            {
                Connection.Open();
                SqlDataReader Reader = Command.ExecuteReader();
                if (Reader.Read())
                {
                    IsFound = true;
                    LicenseClass = (string)Reader["ClassName"];
                    Name = (string)Reader["Name"];
                    LicenseID = (int)Reader["LicenseID"];
                    NationalNo = (string)Reader["NationalNo"];
                    if (Reader["ImagePath"] != DBNull.Value)
                        ImagePath = (string)Reader["ImagePath"];
                    else
                        ImagePath = "";
                    IssueDate = (DateTime)Reader["IssueDate"];
                    IssueReason = (byte)Reader["IssueReason"];

                    if (Reader["Notes"] !=DBNull.Value)
                        Notes = (string)Reader["Notes"];
                    else
                        Notes = "No Notes";

                    IsActive = (bool)Reader["IsActive"];
                    DateOfBirth = (DateTime)Reader["DateOfBirth"];
                    DriverID = (int)Reader["DriverID"];
                    ExpirationDate = (DateTime)Reader["ExpirationDate"];
                    Gender = (byte)Reader["Gendor"];
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
        public static bool IsLicenseStillDetained(int LicenseID)
        {
            bool IsFound = false;
            string Querey = @"select found=1 from DetainedLicenses
                            where LicenseID=@LicenseID and IsReleased=0;";
            SqlConnection Connection = new SqlConnection(Settings.ConnectionString);
            SqlCommand Command=new SqlCommand(Querey, Connection);

            Command.Parameters.AddWithValue("@LicenseID", LicenseID);
            try
            {
                Connection.Open();
                SqlDataReader Reader = Command.ExecuteReader();

                IsFound = Reader.HasRows;

                Reader.Close();
            }catch(Exception ex)
            {
                IsFound=false;
            }
            finally
            {
                Connection.Close( );
            }
            return IsFound;

        }


        public static bool IsThisPersonHasLicenseFromTheSameClass(
            int PersonID, int LicenseClassID, ref int LicenseID)
        {
            bool IsFound = false;
            string Querey = @"select LicenseID from 
                             Licenses inner join Applications on
                             Licenses.ApplicationID=Applications.ApplicationID
                             inner join People on Applications.ApplicantPersonID=
                             People.PersonID 
                             where People.PersonID=@PersonID and Licenses.LicenseClass=@LicenseClassID;

                             ";
            SqlConnection Connection=new SqlConnection(Settings.ConnectionString);  
            SqlCommand Command=new SqlCommand( Querey, Connection);
            Command.Parameters.AddWithValue("@PersonID", PersonID);
            Command.Parameters.AddWithValue("@LicenseClassID", LicenseClassID);

            try
            {
                Connection.Open();
                SqlDataReader Reader = Command.ExecuteReader();
                if (Reader.Read())
                {
                    IsFound = true;
                    LicenseID = (int)Reader["LicenseID"];
                }
                Reader.Close();
            }catch (Exception ex)
            {
                IsFound=false;
            }
            finally
            {
                Connection.Close( );
            }
            return IsFound;
        }

        public static DataTable GetLocalLicensesHistoryForAPerson(int PersonID)
        {
            DataTable dt    = new DataTable();
            string Querey = @"select Licenses.LicenseID ,Applications.ApplicationID,
                           LicenseClasses.ClassName,Licenses.IssueDate,
                           Licenses.ExpirationDate,Licenses.IsActive from 
                           Licenses inner join Applications on Licenses.ApplicationID=
                           Applications.ApplicationID inner join LicenseClasses on 
                           Licenses.LicenseClass=LicenseClasses.LicenseClassID
                           where Applications.ApplicantPersonID= @PersonID and Applications.ApplicationStatus =3;
                           ";

            SqlConnection connection=new SqlConnection(Settings.ConnectionString);
            SqlCommand Command=new SqlCommand(Querey, connection);
            Command.Parameters.AddWithValue("@PersonID", PersonID);
            try
            {
                connection.Open();
                SqlDataReader Reader = Command.ExecuteReader();
                if (Reader.HasRows)
                {
                    dt.Load(Reader);
                }
                Reader.Close();
            }catch(Exception ex)
            {

            }
            finally
            {
                connection.Close();
            }

            return dt;
        }


        public static bool GetLicenseDetailsUsingLicenseID(
            ref string LicenseClass, ref string Name,  int LicenseID,
            ref string NationalNo, ref string ImagePath, ref DateTime IssueDate, ref byte IssueReason,
            ref string Notes, ref bool IsActive, ref DateTime DateOfBirth,
            ref byte Gendor, ref int DriverID, ref DateTime ExpirationDate)
        {
            bool IsFound = false;
            string Querey = @"select LicenseClasses.ClassName,
                            (People.FirstName + ' ' + People.SecondName + ' '
                                                        + People.ThirdName + ' ' + People.LastName) AS Name,
                            People.NationalNo,People.ImagePath,Licenses.IssueDate,
                            Licenses.IssueReason,Licenses.Notes,Licenses.IsActive,People.DateOfBirth,
                            People.Gendor
                            ,Licenses.DriverID,Licenses.ExpirationDate from 
                            Licenses inner join LicenseClasses on 
                            Licenses.LicenseClass=LicenseClasses.LicenseClassID
                            inner join Applications on Licenses.ApplicationID=
                            Applications.ApplicationID inner join People
                            on Applications.ApplicantPersonID=
                            People.PersonID
                            where Licenses.LicenseID=@LicenseID;";
            SqlConnection Connection=new SqlConnection(Settings.ConnectionString);  
            SqlCommand Command=new SqlCommand( Querey, Connection);
            Command.Parameters.AddWithValue("@LicenseID", LicenseID);
            try
            {
                Connection.Open();
                SqlDataReader Reader= Command.ExecuteReader();
                if (Reader.Read()) {

                    IsFound = true;
                    LicenseClass = (string)Reader["ClassName"];
                    Name = (string)Reader["Name"];
                   
                    NationalNo = (string)Reader["NationalNo"];
                    if (Reader["ImagePath"] != DBNull.Value)
                        ImagePath = (string)Reader["ImagePath"];
                    else
                        ImagePath = "";
                    IssueDate = (DateTime)Reader["IssueDate"];
                    IssueReason = (byte)Reader["IssueReason"];

                    if (Reader["Notes"] != DBNull.Value)
                        Notes = (string)Reader["Notes"];
                    else
                        Notes = "No Notes";

                    IsActive = (bool)Reader["IsActive"];
                    DateOfBirth = (DateTime)Reader["DateOfBirth"];
                    DriverID = (int)Reader["DriverID"];
                    ExpirationDate = (DateTime)Reader["ExpirationDate"];
                    Gendor = (byte)Reader["Gendor"];

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

        public static int GetDriverIDByLocalLicenseID(int LocalLicenseID)
        {
            int DriverID = 0;
            string Querey = @"Select DriverID from Licenses where 
                            Licenses.LicenseID=@LocalLicenseID;";
            SqlConnection Connection = new SqlConnection(Settings.ConnectionString);
            SqlCommand Command=new SqlCommand(Querey, Connection);
            Command.Parameters.AddWithValue("@LocalLicenseID", LocalLicenseID);
            try
            {
                Connection.Open();
                SqlDataReader Reader = Command.ExecuteReader();
                if (Reader.Read())
                {
                    DriverID = (int)Reader["DriverID"];
                }
                Reader.Close();
            }catch(Exception ex)
            {
                DriverID=0;
            }
            finally
            {
                Connection.Close();
            }
            return DriverID;
        }

        public static bool GetLicenseFeesUsingLicenseClassID(int LicenseClassID,
            ref Decimal LicenseFees )
        {
            bool IsFound = false;
            string Querey = @"select LicenseClasses.ClassFees 
                           from LicenseClasses
                           where LicenseClasses.LicenseClassID=@LicenseClassID;";
            SqlConnection Connection =new SqlConnection(Settings.ConnectionString);
            SqlCommand Command=new SqlCommand( Querey, Connection);

            Command.Parameters.AddWithValue("@LicenseClassID", LicenseClassID);
            try
            {
                Connection.Open();
                SqlDataReader Reader = Command.ExecuteReader();
                if (Reader.Read())
                {
                    IsFound = true;
                    LicenseFees = (Decimal)Reader["ClassFees"];
                }
                Reader.Close();
            }catch (Exception ex)
            {
                IsFound = false;
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Connection.Close();
            }
            return IsFound;
        }
        public static bool GetLicenseFeesUsingLicenseClassName(string ClassName,
            ref Decimal LicenseFees)
        {
            bool IsFound = false;
            string Querey = @"select LicenseClasses.ClassFees 
                           from LicenseClasses
                           where LicenseClasses.ClassName=@ClassName;";
            SqlConnection Connection = new SqlConnection(Settings.ConnectionString);
            SqlCommand Command = new SqlCommand(Querey, Connection);

            Command.Parameters.AddWithValue("@ClassName", ClassName);
            try
            {
                Connection.Open();
                SqlDataReader Reader = Command.ExecuteReader();
                if (Reader.Read())
                {
                    IsFound = true;
                    LicenseFees = (Decimal)Reader["ClassFees"];
                }
                Reader.Close();
            }
            catch (Exception ex)
            {
                IsFound = false;
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Connection.Close();
            }
            return IsFound;
        }

        public static int GetLicenseClassIDByClassName(string ClassName)
        {
            int ClassID = 0;
            string Querey = @"select LicenseClasses.LicenseClassID
                             from LicenseClasses 
                             where LicenseClasses.ClassName=@ClassName;";

            SqlConnection Connection=new SqlConnection(Settings.ConnectionString);
            SqlCommand Command = new SqlCommand( Querey, Connection);

            Command.Parameters.AddWithValue("@ClassName", ClassName);
            try
            {
                Connection.Open();
                SqlDataReader Reader = Command.ExecuteReader();
                if (Reader.Read())
                {
                    ClassID = (int)Reader["LicenseClassID"];
                }
                Reader.Close();
            }catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }finally 
            { 
                Connection.Close();
            }

            return ClassID;
        }

        public static bool DeActivateLicenseByID(int LicenseID)
        {
            int NumberOfAffectedRows = 0;
            string Querey = @"Update Licenses 
                             set IsActive=0
                             where Licenses.LicenseID=@LicenseID;
                             ";
            SqlConnection Connection= new SqlConnection(Settings.ConnectionString); 
            SqlCommand Command = new SqlCommand(Querey, Connection);
            Command.Parameters.AddWithValue("@LicenseID", LicenseID);

            try
            {
                Connection.Open();
                NumberOfAffectedRows = Command.ExecuteNonQuery();

            }catch(Exception ex)
            {
                NumberOfAffectedRows = 0;
                MessageBox.Show(ex.Message);
            }
            finally
            {
                Connection.Close();
            }
            return NumberOfAffectedRows > 0;
        }

    }
}
