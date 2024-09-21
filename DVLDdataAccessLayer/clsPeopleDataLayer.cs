using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Linq.Expressions;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using System.Drawing;
using System.IO;


namespace DVLDdataAccessLayer
{
    public class PersonDTO
    {
        public int ID { get; set; }
        public string NationalNumber { get; set; }
        public string FullName { get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Gender { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Country { get; set; }

        public string ImagePath { get; set; }

        public PersonDTO(int personID, string nationNo, string fullName, DateTime dateOfBirth, string gender, string phone, string email, string country)
        {
            ID = personID;
            NationalNumber = nationNo;
            FullName = fullName;
            DateOfBirth = dateOfBirth;
            Gender = gender;
            Phone = phone;
            if (email != null)
                Email = email;
            else
                Email = "not found";

            Country = country;
        }
        public PersonDTO() { }
    }

    
    public class UpdatePersonDTO
    {

        public string Address { set; get; }
        public string Phone { set; get; }
        public string Email { set; get; }
        public string ImagePath { set; get; }


    }
    public static class clsPeopleDataLayer
    {
        public static int AddNewPerson(string NationalNumber,
            string FirstName, string SecondName,
           string ThirdName, string LastName, DateTime DateOfBirth,
           string Address, string Phone, string Email, int CountryID,
           int Gender, string ImagePath)
        {

            int InsertedID=-1;

            try {
                using (SqlConnection Connection = new SqlConnection(Settings.ConnectionString))
                {
                    using (SqlCommand Command = new SqlCommand("SP_AddNewPerson", Connection))
                    {

                        Command.CommandType = CommandType.StoredProcedure;


                        Command.Parameters.AddWithValue("@NationalNumber", NationalNumber);
                        Command.Parameters.AddWithValue("@FirstName", FirstName);
                        Command.Parameters.AddWithValue("@SecondName", SecondName);
                        Command.Parameters.AddWithValue("@ThirdName", ThirdName);
                        Command.Parameters.AddWithValue("@LastName", LastName);
                        Command.Parameters.AddWithValue("@DateOfBirth", DateOfBirth);
                        Command.Parameters.AddWithValue("@Address", Address);
                        Command.Parameters.AddWithValue("@Phone", Phone);
                        Command.Parameters.AddWithValue("@Email", Email);
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



                        SqlParameter outputparameter = new SqlParameter("@PersonID", DbType.Int32)
                        {
                            Direction = ParameterDirection.Output
                        };

                        Command.Parameters.Add(outputparameter);


                        Connection.Open();
                        int NumberOfAffectedRows = 0;
                        if ((NumberOfAffectedRows = Command.ExecuteNonQuery()) > 0)
                        {
                            InsertedID = (int)outputparameter.Value;
                        }
                    }
                }

                
            }
            catch (Exception ex)
            {
                InsertedID = -1;
                Settings.AddErrorToEventViewer("Error In Add New Person DataLayer Func",
                    ex.Message);
            }
            


            return InsertedID;


        }


        public static List<PersonDTO> GetPersonsList()
        {
            List<PersonDTO>personsList = new List<PersonDTO>();

            try {
                using (SqlConnection Connection = new SqlConnection(Settings.ConnectionString))
                {
                    using (SqlCommand Command = new SqlCommand("SP_GetPoepleList", Connection))
                    {
                        Command.CommandType = CommandType.StoredProcedure;


                        Connection.Open();
                        SqlDataReader Reader = Command.ExecuteReader();


                        while (Reader.Read())
                        {

                            personsList.Add(new PersonDTO
                            {
                                ID = (int)Reader["PersonID"],
                                NationalNumber = (string)Reader["NationalNO"],
                                FullName = (string)Reader["fullName"],
                                DateOfBirth = (DateTime)Reader["DateOfBirth"],
                                Gender = (string)Reader["Gendor"],
                                Phone = (string)Reader["Phone"],
                                Email = Reader["Email"] == System.DBNull.Value ? "not found" : (string)Reader["Email"],
                                Country = (string)Reader["CountryName"],
                                ImagePath = Reader["ImagePath"] == DBNull.Value?"Not found":(string)Reader["ImagePath"]

                            });

                        }
                    }
                }

                
            }
            catch (Exception ex)
            {
                Settings.AddErrorToEventViewer("Error In Get Person List Data Layer Func",
                    ex.Message);
            }
           

            return personsList;

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
            ref int CountryID,ref DateTime DateOfBirth,ref string Country)
        {
            bool IsFound=false;
            try {
                using (SqlConnection Connection = new SqlConnection(Settings.ConnectionString))
                {
                    using (SqlCommand Command = new SqlCommand("SP_GetPersonByID", Connection))
                    {
                        Command.CommandType = CommandType.StoredProcedure;
                        Command.Parameters.AddWithValue("@PersonID", PersonID);

                        Connection.Open();

                        using (SqlDataReader Reader = Command.ExecuteReader())
                        {
                            if (Reader.Read())
                            {
                                IsFound = true;
                                NationalNumber = (string)Reader["NationalNo"];
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
                                Country = (string)Reader["CountryName"];



                            }
                        }
                    }
                }
            }catch (Exception ex)
            {
                IsFound=false;
                Settings.AddErrorToEventViewer("Error In Find Person DataLayer Func",
                    ex.Message);
            }
           
            return IsFound;
        }
        public static bool FindPersonByNationalNumber(ref int PersonID,  string NationalNumber,
           ref string FirstName, ref string SecondName, ref string ThirdName,
           ref string LastName, ref int Gender, ref string Email,
           ref string Phone, ref string Address, ref string ImagePath,
           ref int CountryID, ref DateTime DateOfBirth,ref string Country)
        {
            bool IsFound = false;
            try {
                using (SqlConnection Connection = new SqlConnection(Settings.ConnectionString))
                {
                    using (SqlCommand Command = new SqlCommand("SP_GetPersonByNationalNo", Connection))
                    {
                        Command.CommandType = CommandType.StoredProcedure;

                        Command.Parameters.AddWithValue("@NationalNumber", NationalNumber);


                        Connection.Open();
                        using (SqlDataReader Reader = Command.ExecuteReader())
                        {
                            if (Reader.Read())
                            {
                                IsFound = true;
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
                                Country = (string)Reader["CountryName"];
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                IsFound = false;
                Settings.AddErrorToEventViewer("Error In Find Person (National No) DataLayer Func",
                ex.Message);

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
            try
            {
                using (SqlConnection Connection = new SqlConnection(Settings.ConnectionString))
                {
                    using (SqlCommand Command = new SqlCommand("SP_UpdatePersonByID", Connection))
                    {

                        Command.CommandType = CommandType.StoredProcedure;

                        Command.Parameters.AddWithValue("@NationalNumber", NationalNumber);
                        Command.Parameters.AddWithValue("@FirstName", FirstName);
                        Command.Parameters.AddWithValue("@SecondName", SecondName);
                        Command.Parameters.AddWithValue("@ThirdName", ThirdName);
                        Command.Parameters.AddWithValue("@LastName", LastName);
                        Command.Parameters.AddWithValue("@Phone", Phone);
                        Command.Parameters.AddWithValue("@Email", Email);
                        Command.Parameters.AddWithValue("@Address", Address);
                        Command.Parameters.AddWithValue("@CountryID", CountryID);
                        Command.Parameters.AddWithValue("@Gender", Gender);
                        Command.Parameters.AddWithValue("@ImagePath", ImagePath);
                        Command.Parameters.AddWithValue("@DateOfBirth", DateOfBirth);
                        Command.Parameters.AddWithValue("@PersonID", PersonID);




                        Connection.Open();
                        NumberOfAffectedRows = Command.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                NumberOfAffectedRows = 0;
                Settings.AddErrorToEventViewer("Error In Update Person DataLayer Func",
                ex.Message);

            }

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