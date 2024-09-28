using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using Serilog;

namespace DVLDdataAccessLayer
{
    public class clsOrdersDataLayer
    {
        public enum enWhatToDo { Cancel=2,Complete=3};
       


        public static int AddNewApplication(int ApplicantPersonID,
            DateTime ApplicationDate,
            int ApplicationTypeID,byte ApplicationStatus,
            DateTime LastStatusDate , Decimal PaidFees,int CreatedByUserID)
        {
            Log.Information("Starting AddNewApplication func in clsOrdersDataLayer");

            int InsertedID = -1;
            try {
                using (SqlConnection Connection = new SqlConnection(Settings.ConnectionString))
                {
                    Connection.Open();
                    Log.Information("Connection Stablished Successfully");

                    using (SqlCommand Command = new SqlCommand("SP_AddNewApplication", Connection))
                    {

                        Command.CommandType = CommandType.StoredProcedure;

                        Command.Parameters.AddWithValue("@ApplicantPersonID", ApplicantPersonID);
                        Command.Parameters.AddWithValue("@ApplicationDate", ApplicationDate);
                        Command.Parameters.AddWithValue("@ApplicationTypeID", ApplicationTypeID);
                        Command.Parameters.AddWithValue("@ApplicationStatus", ApplicationStatus);
                        Command.Parameters.AddWithValue("@LastStatusDate", LastStatusDate);
                        Command.Parameters.AddWithValue("@PaidFees", PaidFees);
                        Command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);

                        SqlParameter outputparam = new SqlParameter("@ApplicationID", DbType.Int32)
                        {
                            Direction = ParameterDirection.Output,
                        };

                        Command.Parameters.Add(outputparam);
                        
                        int NumberOfAffectedRows = 0;
                        if ((NumberOfAffectedRows = Command.ExecuteNonQuery()) > 0)
                        {
                            Log.Information($"Number of affected rows : {NumberOfAffectedRows}");

                            InsertedID = (int)outputparam.Value;
                        }
                    }
                }
                Log.Information($"Application Added successfully with ID : {InsertedID}");

            }catch (Exception ex)
            {
                Log.Error(ex, "Error Exception in AddNewApplication func clsOrdersDatalayer");
                InsertedID=-1;
            }

            return InsertedID;

        }

        public static bool IsThisPersonIDHasAnActiveApplicationForThisLicenseTypeID(int PersonID
            ,int LicenseTypeID,ref int ApplicationID)
        {
            Log.Information($"starting IsThisPersonIDHasAnActiveApplicationForThisLicenseTypeID with PersonID :{PersonID} , " +
                $"and LicenseTypeID : {LicenseTypeID}");
                ApplicationID = -1;
            try {

                using (SqlConnection connection = new SqlConnection(Settings.ConnectionString))
                {
                    connection.Open();

                    Log.Information("Connection to database established successfully");

                    using (SqlCommand Command = new SqlCommand("SP_GetApplicationIDByPersonIDAndLicenseTypeID", connection))
                    {

                        Command.CommandType = CommandType.StoredProcedure;

                        Command.Parameters.AddWithValue("@PersonID", PersonID);
                        Command.Parameters.AddWithValue("@LicenseTypeID", LicenseTypeID);
                        
                        using (SqlDataReader Reader = Command.ExecuteReader())
                        {

                            if (Reader.Read())
                            {
                                ApplicationID = (int)Reader["ApplicationID"];
                                Log.Information($"Reader reads ApplicationID : {ApplicationID}");
                            }
                        }
                    }
                }
                Log.Information("IsThisPersonIDHasAnActiveApplicationForThisLicenseTypeID Executed successfully");
            }catch(Exception ex)
            {
                Log.Information(ex, "Error in IsThisPersonIDHasAnActiveApplicationForThisLicenseTypeID");
                ApplicationID=-1;
            }
           

            return ApplicationID != -1;


        }

        public static bool FindApplicationByID(int ApplicationID,ref int PersonID,
            ref DateTime AppDate,
            ref int OrderNameID,ref byte ApplicationStatusID)
        {
            Log.Information($"starting FindApplicationByID in clsOrdersDatalayer using ApplicationID : {ApplicationID}");
            bool IsFound = false;
            try { 

                using (SqlConnection connection = new SqlConnection(Settings.ConnectionString))
                {
                    connection.Open();
                    Log.Information("Connection to database established successfully");

                    using (SqlCommand Command = new SqlCommand("SP_FindApplicationByApplicationID", connection))
                    {

                        Command.CommandType = CommandType.StoredProcedure;
                        Command.Parameters.AddWithValue("@ApplicationID", ApplicationID);

                        
                        using (SqlDataReader Reader = Command.ExecuteReader())
                        {
                            if (Reader.Read())
                            {
                                Log.Information("Reader is reading");

                                IsFound = true;
                                PersonID = (int)Reader["ApplicantPersonID"];
                                AppDate = (DateTime)Reader["ApplicationDate"];
                                OrderNameID = (int)Reader["ApplicationTypeID"];
                                ApplicationStatusID = (byte)Reader["ApplicationStatus"];
                            }
                        }
                    }
                }

                Log.Information("FindApplicationByID executed successfully");

            }catch(Exception ex)
            {
                Log.Error(ex, "Error (Exception) in FindApplicationByID");
                IsFound = false;
            }
           

            return IsFound;
        }

        public static bool GetStatusName(int OrderCaseID,ref string status)
        {
            bool IsFound = false;
            string Querey = @"select OrderCase.OrderCase from OrderCase
                             where OrderCase.OrderCaseID=@OrderCaseID;";
            SqlConnection Connection= new SqlConnection(Settings.ConnectionString);
            SqlCommand Command = new SqlCommand(Querey, Connection);

            Command.Parameters.AddWithValue("@OrderCaseID", OrderCaseID);

            try
            {
                Connection.Open();
                SqlDataReader Reader = Command.ExecuteReader();
                if (Reader.Read())
                {
                    IsFound = true;
                    status = (string)Reader["OrderCase"];
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

        public static bool GetServiceNameAndFees(int OrderNameID,ref string ServiceName,ref Decimal Fees)
        {
            bool IsFound = false;
            string Querey = @"select ApplicationFees,
                            ApplicationTypeTitle from 
                            ApplicationTypes
                            where ApplicationTypeID=@OrderNameID;";

            SqlConnection Connection= new SqlConnection(Settings.ConnectionString); 
            SqlCommand Command = new SqlCommand( Querey, Connection);

            Command.Parameters.AddWithValue("@OrderNameID", OrderNameID);

            try
            {
                Connection.Open();
                SqlDataReader Reader = Command.ExecuteReader();
                if (Reader.Read())
                {
                    IsFound = true;
                    float.TryParse(Reader["ApplicationFees"].ToString(), out float fees);
                    Fees = (Decimal)Reader["ApplicationFees"];
                    ServiceName = (string)Reader["ApplicationTypeTitle"];
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

        public static bool EditPassedTests(int ApplicationID)
        {
            int NumberOfAffectedRows = 0;
            string Querey = @"update Orders 
                             set PassedTests +=1
                             where OrderID=@ApplicationID;";
            SqlConnection Connection= new SqlConnection(Settings.ConnectionString); 
            SqlCommand Command = new SqlCommand(Querey, Connection);
            Command.Parameters.AddWithValue("@ApplicationID",ApplicationID);
           

            try
            {
                Connection.Open();
                NumberOfAffectedRows = Command.ExecuteNonQuery();
            }catch (Exception ex)
            {
                NumberOfAffectedRows = 0;
            }finally { Connection.Close(); }

            return NumberOfAffectedRows > 0;
        }

        public static bool GetApplicationFeesUsingName(string ServiceName , ref decimal Fees)
        {
            Fees= 0;
            string Querey = @"select ApplicationTypes.ApplicationFees from ApplicationTypes
                             where ApplicationTypes.ApplicationTypeTitle =  
                            @ServiceName;";

            SqlConnection connection = new SqlConnection(Settings.ConnectionString);
            SqlCommand Command = new SqlCommand( Querey, connection);
            Command.Parameters.AddWithValue("@ServiceName", ServiceName);

            try
            {
                connection.Open();
                SqlDataReader Reader = Command.ExecuteReader();
                if (Reader.Read())
                {
                    //float.TryParse(Reader["ApplicationFees"].ToString(), out float fees);
                    
                    Fees = Reader.GetDecimal(Reader.GetOrdinal("ApplicationFees")); 
                    Reader.Close();
                }
            }catch (Exception ex)
            {
                Fees = 0;
            }
            finally
            {
                connection.Close();
            }
            return Fees > 0;
        }

       
        public static DataTable GetAllApplicationForInternationalLicenses()
        {
            DataTable dataTable = new DataTable();
            string Querey = @"select * from InternationalLicenses;";
            SqlConnection Connection =new SqlConnection(Settings.ConnectionString);
            SqlCommand Command = new SqlCommand( Querey, Connection);
            try
            {
                Connection.Open();
                SqlDataReader Reader = Command.ExecuteReader();
                if (Reader.HasRows)
                {
                    dataTable.Load(Reader);
                }
                Reader.Close();
            }catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }finally { Connection.Close(); }
            return dataTable;
        }

        public static async Task<bool> UpdateApplicationStatus(int ApplicationID,enWhatToDo whattodo)
        {
            Log.Information($"Start UpdateApplicationStatus func in clsOrdersDataLayer with " +
                $"ApplicationID : {ApplicationID} , {whattodo}");
            
            int NumberOfAffectedRows = 0;
            try {

                using (SqlConnection Connection = new SqlConnection(Settings.ConnectionString))
                {   
                    await Connection.OpenAsync();

                    Log.Information("Connection to database stablished successfully");

                    using (SqlCommand Command = new SqlCommand("SP_UpdateApplicationStatus", Connection))
                    {

                        Command.CommandType = CommandType.StoredProcedure;
                        Command.Parameters.AddWithValue("@ApplicationID", ApplicationID);
                        Command.Parameters.AddWithValue("@StatusNumber", (int)whattodo);
                        
                        NumberOfAffectedRows = await Command.ExecuteNonQueryAsync();
                    }
                }

            }
            catch (Exception ex)
            {
                Log.Error(ex, "Error in (UpdateApplicationStatus)");
                NumberOfAffectedRows = 0;
            }
            

            return NumberOfAffectedRows > 0;
        }

        




    }
}
