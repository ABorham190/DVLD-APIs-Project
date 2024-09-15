using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Windows.Forms;

namespace DVLDdataAccessLayer
{
    public static class clsUsersDataLayer
    {
        public class UserDTO
        {
            int ID { get; set; }
            public string Name { get; set; }
            public string UserName { get; set; }
            public bool IsActive { get; set; }

            public UserDTO(int id,string name,bool isactive,string username) 
            {

                this.ID = id;
                this.Name = name;
                this.IsActive = isactive;
                this.UserName = username;
                
            
            }
        }

        public class AddNewUserDTO
        {

            public int PersonID { get; set; }
            public string UserName { get; set; }
            public string Password { get; set; }
            public bool IsActive { get; set; }



        }

        public class UpdateUserDTO
        {

           
            public string UserName { get; set; }
            public string Password { get; set; }
            public bool IsActive { get; set; }

        }
        static public int AddNewUser (int PersonID,string UserName,
            string Password,bool IsActive)
        {
            int InsertedID = -1;
            string HashedPassword = Settings.ComputeHash(Password);
            try
            {
                using (SqlConnection Connection = new SqlConnection(Settings.ConnectionString))
                {


                    using (SqlCommand Command = new SqlCommand("SP_AddNewUser", Connection))

                    {
                        Command.CommandType = CommandType.StoredProcedure;

                        Command.Parameters.AddWithValue("@PersonID", PersonID);
                        Command.Parameters.AddWithValue("@UserName", UserName);
                        Command.Parameters.AddWithValue("@Password", HashedPassword);
                        Command.Parameters.AddWithValue("@IsActive", IsActive);

                        SqlParameter outputparameter = new SqlParameter("@UserID", DbType.Int32)
                        {
                            Direction = ParameterDirection.Output,
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
            catch (Exception ex) {

                InsertedID = -1;
                Settings.AddInfoToEventViewer("Error in add new UserFunc "+ ex.Message);
            
            }
            return InsertedID;
        }
        static public List<UserDTO> GetUsersDTOs()
        {
            List<UserDTO>UsersList=new List<UserDTO>();
            try
            {
                using (SqlConnection Connection = new SqlConnection(Settings.ConnectionString))
                {
                    Connection.Open();
                    using (SqlCommand Command = new SqlCommand("SP_GetUsersListdto", Connection))
                    {

                        Command.CommandType = CommandType.StoredProcedure;
                        using (SqlDataReader Reader = Command.ExecuteReader())
                        {
                            while (Reader.Read())
                            {
                                UsersList.Add(new UserDTO((int)Reader["UserID"], (string)Reader["FullName"],
                                    (bool)Reader["IsActive"], (string)Reader["UserName"]));
                            }
                        }

                    }
                }

            }
            catch (Exception ex) 
            {
                Settings.AddErrorToEventViewer("Error in GetUsersDTOs", ex.Message);
            }

            return UsersList;
        }
       
        static public bool DeleteUser(int UserID)
        {
            int NumberOfAffectedRows = 0;
            try
            {
                using (SqlConnection Connection = new SqlConnection(Settings.ConnectionString))
                {
                    using (SqlCommand Command = new SqlCommand("SP_DeleteUser", Connection))
                    {
                        Connection.Open();
                        Command.CommandType= CommandType.StoredProcedure;
                        Command.Parameters.AddWithValue("@UserID", UserID);

                        
                        NumberOfAffectedRows = Command.ExecuteNonQuery();
                    }
                }
                
            }catch(Exception ex)
            {
                NumberOfAffectedRows = 0;
                Settings.AddErrorToEventViewer("Error in DeleteUser func ", ex.Message);
            }
            
            return NumberOfAffectedRows > 0;
        }
        static public bool FindUserByUserID(int UserID,ref int PersonID,
            ref string UserName,ref string Password,ref bool IsActive)
        {
            bool IsFound = false;
            using (SqlConnection Connection = new SqlConnection(Settings.ConnectionString))
            {

                string Querey = @"select * from Users where UserID=@UserID";
                using (SqlCommand Command = new SqlCommand(Querey, Connection))
                {
                    Command.Parameters.AddWithValue("@UserID", UserID);
                    try
                    {
                        Connection.Open();
                        using (SqlDataReader Reader = Command.ExecuteReader())
                        {
                            if (Reader.Read())
                            {
                                IsFound = true;
                                PersonID = (int)Reader["PersonID"];
                                Password = (string)Reader["Password"];
                                UserName = (string)Reader["UserName"];
                                IsActive = (bool)Reader["IsActive"];
                            }
                        }

                    }
                    catch (Exception ex)
                    {
                        IsFound = false;
                        Settings.AddErrorToEventViewer("Error In Find User By UserID Datalayer Func", ex.Message);
                    }
                }
            }

            return IsFound;
        }
        static public bool FindUserByUserNameAndPassword(ref int UserID, ref int PersonID,
             string UserName, string Password, ref bool IsActive)
        {
            bool IsFound = false;
            string HashedPassword=Settings.ComputeHash(Password);
            try
            {
                using (SqlConnection Connection = new SqlConnection(Settings.ConnectionString))
                {

                    
                    using (SqlCommand Command = new SqlCommand("SP_FindUserByUserNameAndPassword",
                        Connection))
                    {
                        Command.Parameters.AddWithValue("@UserName", UserName);
                        Command.Parameters.AddWithValue("@Password", HashedPassword);

                        Command.CommandType = CommandType.StoredProcedure;
                        try
                        {
                            Connection.Open();
                            using (SqlDataReader Reader = Command.ExecuteReader())
                            {
                                if (Reader.Read())
                                {
                                    IsFound = true;
                                    PersonID = (int)Reader["PersonID"];
                                    UserID = (int)Reader["UserID"];
                                    IsActive = (bool)Reader["IsActive"];
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            IsFound = false;
                            Settings.AddErrorToEventViewer("Error in FindUserByUserNameAndPassword", ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                IsFound = false;
                Settings.AddErrorToEventViewer("Error in findUserByUserNameAndPassword func",ex.Message);
            }


            return IsFound;
        }

        static public bool UpdateUserIsActive(int UserID,bool IsActive,
            string UserName,string Password)
        {
            int NumberOfAffectedRows = 0;
            string HashedPassword= Settings.ComputeHash(Password);
            using (SqlConnection Connection = new SqlConnection(Settings.ConnectionString))
            {

                using (SqlCommand Command = new SqlCommand("SP_UpdateUser", Connection))
                {
                    Command.CommandType = CommandType.StoredProcedure;

                    Command.Parameters.AddWithValue("@IsActive", IsActive);
                    Command.Parameters.AddWithValue("@UserID", UserID);
                    Command.Parameters.AddWithValue("@Password", HashedPassword);
                    Command.Parameters.AddWithValue("@UserName", UserName);
                    try
                    {
                        Connection.Open();
                        NumberOfAffectedRows = Command.ExecuteNonQuery();

                    }
                    catch (Exception ex)
                    {
                        NumberOfAffectedRows = 0;
                        Settings.AddErrorToEventViewer("Error In Update User DataLayer Func", ex.Message);
                    }
                }
            }

            return NumberOfAffectedRows > 0;

        }


        static public bool GetUserNameByUserID(int UserID,ref string UserName)
        {
            bool IsFound = false;
            try
            {

                using (SqlConnection connection = new SqlConnection(Settings.ConnectionString))
                {
                    using (SqlCommand Command = new SqlCommand("SP_GetUserNameByUserID", connection))
                    {

                        Command.CommandType = CommandType.StoredProcedure;

                        Command.Parameters.AddWithValue("@UserID", UserID);

                        connection.Open();
                        using (SqlDataReader Reader = Command.ExecuteReader())
                        {
                            if (Reader.Read())
                            {

                                IsFound = true;
                                UserName = (string)Reader["username"];

                            }
                        }
                    }
                }

            }
            catch (Exception ex) 
            {
                
                IsFound = false;
            
            }
           
            return IsFound;
        }

        public static bool IsUserExists(int UserID)
        {
            bool IsFound = false;

            try
            {
                using (SqlConnection Connection=new SqlConnection(Settings.ConnectionString))
                {
                    Connection.Open();
                    using (SqlCommand Command=new SqlCommand("SP_IsUserExist", Connection))
                    {
                        Command.CommandType = CommandType.StoredProcedure;
                        Command.Parameters.AddWithValue("@UserID", UserID);

                        using (SqlDataReader Reader = Command.ExecuteReader())
                        {
                            IsFound = Reader.HasRows;
                        }
                    }
                }
            }catch(Exception ex)
            {
                IsFound = false;
                Settings.AddErrorToEventViewer("Error in IsUserExists func " , ex.Message);
             }

            return IsFound;
        }

        
        
    }
}
