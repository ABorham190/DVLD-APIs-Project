using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using System.Linq.Expressions;
using System.Diagnostics;
using System.Windows.Forms;

namespace DVLDdataAccessLayer
{
    public static class clsPeopleDataLayer
    {
        public static int AddNewPerson(string NationalNumber,
            string FirstName, string SecondName,
           string ThirdName, string LastName, DateTime DateOfBirth,
           string Address, string Phone, string Email, int CountryID,
           int Gender, string ImagePath)
        {

            int InsertedID=-1;
            //int CountryID = Settings.GetCountryID(Country);
            string Querey = @"insert into People 
                          (
                           NationalNo,FirstName,SecondName, 
                           ThirdName,LastName,DateOfBirth,Gendor,
                           Address,Phone,Email,NationalityCountryID,ImagePath
                          )values(@NationalNumber,@FirstName,@SecondName
                          ,@ThirdName,
                          @LastName,@DateOfBirth,@Gender,@Address,@Phone,@Email
                          ,@CountryID,@ImagePath);
                           select scope_identity();";


            SqlConnection Connection = new SqlConnection(Settings.ConnectionString);
            SqlCommand Command = new SqlCommand(Querey, Connection);
            Command.Parameters.AddWithValue("@NationalNumber", NationalNumber);
            Command.Parameters.AddWithValue("@FirstName", FirstName);
            Command.Parameters.AddWithValue("@SecondName", SecondName);
            Command.Parameters.AddWithValue("@ThirdName", ThirdName);
            Command.Parameters.AddWithValue("@LastName", LastName);
            Command.Parameters.AddWithValue("@DateOfBirth", DateOfBirth);
            Command.Parameters.AddWithValue("@Address", Address);
            Command.Parameters.AddWithValue("@Phone", Phone);
            Command.Parameters.AddWithValue("@Email", Email);
            //Command.Parameters.AddWithValue("@Country", Country);
            Command.Parameters.AddWithValue("@Gender", Gender);
            Command.Parameters.AddWithValue("@CountryID", CountryID);

            if (ImagePath != "")
            {
                Command.Parameters.AddWithValue("@ImagePath", ImagePath);
            }
            else
            {
                Command.Parameters.AddWithValue("@ImagePath", System.DBNull.Value);
            }


            try
            {
                Connection.Open();
                object Result = Command.ExecuteScalar();
                if (Result != null&&int.TryParse(Result.ToString(),out int ID))
                {
                    InsertedID= ID;
                }

                
            }
            catch (Exception ex)
            {
                InsertedID = -1;
                Settings.AddErrorToEventViewer("Error In Add New Person DataLayer Func",
                    ex.Message);
            }
            finally { Connection.Close(); }


            return InsertedID;


        }


        public static DataTable GetPersonsList()
        {
            DataTable dt = new DataTable();

            string Querey = @"Select PersonID,NationalNo,
                             FirstName,secondName,ThirdName,LastName, 
                              DateOfBirth,Gendor =
                              case
                              when Gendor=1 then 'Male'
                              when Gendor=0 then 'Female'
                              end,
                             Phone,Email,Countries.CountryName from People inner join Countries on
                             People.NationalityCountryID=Countries.CountryID
                             Order by PersonID desc;";
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
            }
            catch (Exception ex)
            {
                Settings.AddErrorToEventViewer("Error In Get Person List Data Layer Func",
                    ex.Message);
            }
            finally { Connection.Close(); }

            return dt;

        }


        public static bool IsPersonExistInDatabase(string NationalNumber)
        {
            bool IsFound = false;
            string Querey = @"select Found=1 from People 
                         where NationalNo=@NationalNumber";
            SqlConnection Connection = new SqlConnection(Settings.ConnectionString);
            SqlCommand Command = new SqlCommand(Querey, Connection);
            Command.Parameters.AddWithValue("@NationalNumber", NationalNumber);
            try
            {
                Connection.Open();
                SqlDataReader Reader = Command.ExecuteReader();
                if (Reader.HasRows)
                {
                    IsFound = true;
                }
            }
            catch(Exception ex)
            {
                IsFound = false;
                Settings.AddErrorToEventViewer("Error In Is Person Exist by (National Number) Datalayer Func",
                    ex.Message);
            }
            finally
            {
                Connection.Close();
            }

            return IsFound;
        }

        public static bool FindPerson(int PersonID,ref string NationalNumber,
            ref string FirstName,ref string SecondName,ref string ThirdName,
            ref string LastName,ref int Gender,ref string Email,
            ref string Phone,ref string Address,ref string ImagePath,
            ref int CountryID,ref DateTime DateOfBirth)
        {
            bool IsFound=false;
            string Querey = "Select * from People where PersonID=@PersonID";
            SqlConnection Connection=new SqlConnection(Settings.ConnectionString);
            SqlCommand Command = new SqlCommand( Querey, Connection);
            Command.Parameters.AddWithValue("@PersonID", PersonID);
            try
            {
                Connection.Open();
                SqlDataReader Reader = Command.ExecuteReader();
                if (Reader.Read())
                {
                    IsFound = true;
                    NationalNumber =(string) Reader["NationalNo"];
                    FirstName = (string)Reader["FirstName"];
                    SecondName = (string)Reader["SecondName"];
                    ThirdName = (string)Reader["ThirdName"];
                    LastName = (string)Reader["LastName"];
                    Gender = (byte)Reader["Gendor"];
                    Address = (string)Reader["Address"];
                    if (Reader["Email"] != null)
                    {
                        Email = (string)Reader["Email"];
                    }
                    else
                    {
                        Email = "";
                    }
                    Phone = (string)Reader["Phone"];
                    CountryID = (int)Reader["NationalityCountryID"];

                    if (Reader["ImagePath"] != System.DBNull.Value)
                    {
                        ImagePath = (string)Reader["ImagePath"];
                    }
                    else
                    {
                        ImagePath = "";
                    }
                    
                    
                    DateOfBirth = (DateTime)Reader["DateOfBirth"];
                   


                }
            }catch (Exception ex)
            {
                IsFound=false;
                Settings.AddErrorToEventViewer("Error In Find Person DataLayer Func",
                    ex.Message);
            }
            finally
            {
                Connection.Close();
            }
            return IsFound;
        }
        public static bool FindPersonByNationalNumber(ref int PersonID,  string NationalNumber,
           ref string FirstName, ref string SecondName, ref string ThirdName,
           ref string LastName, ref int Gender, ref string Email,
           ref string Phone, ref string Address, ref string ImagePath,
           ref int CountryID, ref DateTime DateOfBirth)
        {
            bool IsFound = false;
            string Querey = "Select * from People where NationalNo = @NationalNumber";
            SqlConnection Connection = new SqlConnection(Settings.ConnectionString);
            SqlCommand Command = new SqlCommand(Querey, Connection);
            Command.Parameters.AddWithValue("@NationalNumber", NationalNumber);
            try
            {
                Connection.Open();
                SqlDataReader Reader = Command.ExecuteReader();
                if (Reader.Read())
                {
                    IsFound = true;
                    //NationalNumber = (string)Reader["NationalNumber"];
                    PersonID = (int)Reader["PersonID"];
                    FirstName = (string)Reader["FirstName"];
                    SecondName = (string)Reader["SecondName"];
                    ThirdName = (string)Reader["ThirdName"];
                    LastName = (string)Reader["LastName"];
                    Gender = (byte)Reader["Gendor"];
                    Address = (string)Reader["Address"];
                    if (Reader["Email"] != null)
                    {
                        Email = (string)Reader["Email"];
                    }
                    else
                    {
                        Email = "";
                    }
                    Phone = (string)Reader["Phone"];
                    CountryID = (int)Reader["NationalityCountryID"];

                    if (Reader["ImagePath"] != System.DBNull.Value)
                    {
                        ImagePath = (string)Reader["ImagePath"];
                    }
                    else
                    {
                        ImagePath = "";
                    }


                    DateOfBirth = (DateTime)Reader["DateOfBirth"];



                }
            }
            catch (Exception ex)
            {
                IsFound = false;
                Settings.AddErrorToEventViewer("Error In Find Person (National No) DataLayer Func",
                ex.Message);

            }
            finally
            {
                Connection.Close();
            }
            return IsFound;
        }


        public static bool DeletePerson(int PersonID)
        {
            int NumberOfAffectedRows = 0;
            string Querey = @"Delete from People where PersonID=@PersonID";
            SqlConnection Connection=new SqlConnection(Settings.ConnectionString);
            SqlCommand Command=new SqlCommand(Querey,Connection);
            Command.Parameters.AddWithValue("@PersonID", PersonID);

            try
            {
                Connection.Open();
                NumberOfAffectedRows = Command.ExecuteNonQuery();
            }catch (Exception ex)
            {
                NumberOfAffectedRows=0;
                Settings.AddErrorToEventViewer("Error In Delete Person DataLayer Func",
                ex.Message);

            }
            finally { Connection.Close(); }
            return NumberOfAffectedRows > 0;
        }
        

        public static bool UpdatePersonInfo(int PersonID,string NationalNumber,
            string FirstName,
            string SecondName,string ThirdName,string LastName,string Phone,
            string Email,string Address,int CountryID,int Gender,string ImagePath
            ,DateTime DateOfBirth)
        {
            int NumberOfAffectedRows = 0;
            //int CountryID = Settings.GetCountryID(Country);
            string Querey = @"Update People 
                          set NationalNo=@NationalNumber,FirstName=@FirstName,
                          SecondName=@SecondName,
                          ThirdName=@ThirdName,LastName=@LastName,
                          Phone=@Phone,Email=@Email,Address=@Address,
                          NationalityCountryID=@CountryID,Gendor=@Gender,
                          ImagePath=@ImagePath,DateOfBirth=@DateOfBirth 
                          where PersonID=@PersonID;";

            SqlConnection Connection= new SqlConnection(Settings.ConnectionString); 
            SqlCommand Command=new SqlCommand( Querey,Connection);
            Command.Parameters.AddWithValue("@NationalNumber",NationalNumber);
            Command.Parameters.AddWithValue("@FirstName",FirstName);
            Command.Parameters.AddWithValue("@SecondName",SecondName);
            Command.Parameters.AddWithValue("@ThirdName",ThirdName);
            Command.Parameters.AddWithValue("@LastName",LastName);
            Command.Parameters.AddWithValue("@Phone",Phone);
            Command.Parameters.AddWithValue("@Email",Email);
            Command.Parameters.AddWithValue("@Address",Address);
            Command.Parameters.AddWithValue("@CountryID",CountryID);
            Command.Parameters.AddWithValue("@Gender",Gender);
            Command.Parameters.AddWithValue("@ImagePath",ImagePath);
            Command.Parameters.AddWithValue("@DateOfBirth",DateOfBirth);
            Command.Parameters.AddWithValue("@PersonID", PersonID);

            try
            {
                Connection.Open();
                NumberOfAffectedRows = Command.ExecuteNonQuery();
            }catch (Exception ex)
            {
                NumberOfAffectedRows = 0;
                Settings.AddErrorToEventViewer("Error In Update Person DataLayer Func",
                ex.Message);

            }
            finally { Connection.Close(); }

            return NumberOfAffectedRows > 0;
        }

        public static bool IsPersonAuser(int PersonID)
        {
            bool IsFound = false;
            string Querey = "select found=1 from Users where PersonID=@ID";
            SqlConnection Connection=new SqlConnection(Settings.ConnectionString);
            SqlCommand Command=new SqlCommand(Querey,Connection);
            Command.Parameters.AddWithValue("@ID", PersonID);
            try
            {
                Connection.Open();
                SqlDataReader Reader = Command.ExecuteReader();
                IsFound = Reader.HasRows;
                Reader.Close();
            }catch(Exception ex)
            {
                IsFound = false;
                Settings.AddErrorToEventViewer("Error In Is Person a User DataLayer Func",
                   ex.Message);

            }
            finally
            {
                Connection.Close();
            }
            return IsFound;

        }

        public static bool GetPersonFullName(int PersonID,ref string FullName)
        {
            bool IsFound = false;
            string Querey = @"select (People.FirstName+' '+People.SecondName+' '
                            +People.ThirdName+' '+People.LastName)
                            as fullName from People where People.PersonID=@PersonID;
                            ";

            SqlConnection connection=new SqlConnection(Settings.ConnectionString);
            SqlCommand Command=new SqlCommand( Querey,connection);
            Command.Parameters.AddWithValue("@PersonID", PersonID);

            try
            {
                connection.Open();
                SqlDataReader Reader = Command.ExecuteReader();
                if (Reader.Read())
                {
                    IsFound = true;
                    FullName = (string)Reader["fullName"];
                }
                Reader.Close();
            }catch (Exception ex)
            {
                IsFound = false;
                Settings.AddErrorToEventViewer("Error In Get Person Full Name DataLayer Func",
                ex.Message);

            }
            finally
            {
                connection.Close();
            }
            return IsFound;
        }
        
        public static bool GetPersonIDUsingLDLAppID(int LDLAppID,ref int ApplicantID)
        {   
            bool IsFound = false;
            using (SqlConnection Connection = new SqlConnection(Settings.ConnectionString))
            { 

                string Querey = @"select Applications.ApplicantPersonID from 
                           Applications inner join LocalDrivingLicenseApplications
                           on Applications.ApplicationID=
                           LocalDrivingLicenseApplications.ApplicationID
                           where LocalDrivingLicenseApplicationID=@LDLAppID;
                           ";

                using (SqlCommand Command = new SqlCommand(Querey, Connection)) {
                    Command.Parameters.AddWithValue("@LDLAppID", LDLAppID);
                    

                        try
                        {
                            Connection.Open();
                            using (SqlDataReader Reader = Command.ExecuteReader())
                            {
                                if (Reader.Read())
                                {
                                    IsFound = true;
                                    ApplicantID = (int)Reader["ApplicantPersonID"];
                                    Reader.Close();
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            IsFound = false;
                            Settings.AddErrorToEventViewer("Error In Get PersonID Using LDLAppID DataLayer Func",
                            ex.Message);

                        }
                    
                }
            }
            return IsFound;
        }

        public static bool GetPersonIDUsingNationalNo(string NationalNo,ref int PersonID)
        {
            bool IsFound = false;
            using (SqlConnection Connection = new SqlConnection(Settings.ConnectionString))
            {



                string Querey = @"Select People.PersonID from People 
                              where People.NationalNo=@NationalNo;";

                using (SqlCommand Command = new SqlCommand(Querey, Connection))
                {
                    Command.Parameters.AddWithValue("@NationalNo", NationalNo);
                    try
                    {
                        Connection.Open();
                        using (SqlDataReader Reader = Command.ExecuteReader())
                        {
                            if (Reader.Read())
                            {
                                IsFound = true;
                                PersonID = (int)Reader["PersonID"];
                            }
                            Reader.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        IsFound = false;
                        Settings.AddErrorToEventViewer("Error In GetPersonIDUsingNationalNo Data Layer Func",
                            ex.Message);
                    }
                }
            }
            return IsFound;
        }
        public static int GetPersonIDUsingLicenseID(int LicenseID)
        {
            int PersonID = 0;
            using (SqlConnection Connection = new SqlConnection(Settings.ConnectionString))
            {

                string Querey = @"select Drivers.PersonID from 
                            Drivers inner join Licenses on 
                            Licenses.DriverID=Drivers.DriverID
                            where LicenseID=@LicenseID;
                            ";
                using (SqlCommand Command = new SqlCommand(Querey, Connection))
                {
                    Command.Parameters.AddWithValue("@LicenseID", LicenseID);
                    try
                    {
                        Connection.Open();
                        using (SqlDataReader Reader = Command.ExecuteReader())
                        {
                            if (Reader.Read())
                            {
                                PersonID = (int)Reader["PersonID"];
                            }
                            Reader.Close();
                        }
                    }
                    catch (Exception ex)
                    {
                        PersonID = 0;
                        Settings.AddErrorToEventViewer("Error In Get Person Id Using License Id DataLayer Func",
                            ex.Message);
                    }
                }
            }

            return PersonID;
        }

    }
}