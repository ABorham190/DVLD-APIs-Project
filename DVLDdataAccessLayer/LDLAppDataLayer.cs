using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using Serilog;

namespace DVLDdataAccessLayer
{
    public  class LDLAppDataLayer
    {
        
        public class LDLAppDTO
        {
            public int LDLAppID { get; set; }
            public string LicenseClass { get; set; }
            public string FullName { get; set; }
            public DateTime ApplicationDate { get; set; }
            public int PassedTests { get; set; }
            public string Status { get; set; }


        }

        public static async Task<int> AddNewLDLApp(int AppID,int LicenseClassID)
        {
            Log.Information("Starting execution (AddNewLDLApp) with AppID : {@AppID}", AppID);
            int InsertedID = -1;
            try
            {
                
                using (SqlConnection Connection = new SqlConnection(Settings.ConnectionString))
                {

                    Connection.Open();

                    Log.Information("Connection to database stablished successfully");

                    using (SqlCommand Command = new SqlCommand("SP_AddNewLocalDrivingLicenseApp", Connection))
                    {

                        Command.CommandType = CommandType.StoredProcedure;

                        Command.Parameters.AddWithValue("@ApplicationID", AppID);
                        Command.Parameters.AddWithValue("@LicenseClassID", LicenseClassID);

                        SqlParameter outPutParam = new SqlParameter("LDLAID", DbType.Int32)
                        {
                            Direction = ParameterDirection.Output
                        };
                        Command.Parameters.Add(outPutParam);


                        int NumberOfAffectedRow = 0;
                        if ((NumberOfAffectedRow =await Command.ExecuteNonQueryAsync()) > 0)
                        {
                            Log.Information("Number Of Affected Rows : {@NumberOfAffectedRow}", NumberOfAffectedRow);

                            InsertedID = (int)outPutParam.Value;
                        }


                    }
                }

                Log.Information("LDLApp added successfully with ID : {@ID}", InsertedID);
            }
            catch (Exception ex)
            {

                Log.Error("Error Occured while executing (SP_AddNewLocalDrivingLicenseApp)");
                InsertedID = -1;
                
            }
                
               
            
            return InsertedID;

        }

        public static async Task< List<LDLAppDTO>> GetAllLDLApps()
        {
            Log.Information("Starting execution of GetAllLDLApps in LDLAppDataLayer");
            
            List<LDLAppDTO>ldlapplist=new List<LDLAppDTO>();
            try
            {
                using (SqlConnection Connection = new SqlConnection(Settings.ConnectionString))
                {

                    using (SqlCommand Command = new SqlCommand("SP_GetAllLDLAppWithDetails", Connection))
                    {
                        Command.CommandType = CommandType.StoredProcedure;

                        Connection.Open();

                        Log.Information("Connection to database Stablished successfully");

                        using (SqlDataReader Reader = await Command.ExecuteReaderAsync())
                        {
                            while (await Reader.ReadAsync())
                            {
                                ldlapplist.Add(new LDLAppDTO
                                {
                                    LDLAppID = (int)Reader["LDLAppID"],
                                    LicenseClass = (string)Reader["LicenseClass"],
                                    FullName = (string)Reader["FullName"],
                                    ApplicationDate = (DateTime)Reader["ApplicationDate"],
                                    PassedTests = (int)Reader["PassedTests"],
                                    Status = (string)Reader["Status"]
                                });

                            }
                        }
                    }
                }

                Log.Information("(SP_GetAllLDLAppWithDetails) executed successfully and ldlapplist contain {@count} items", ldlapplist.Count);
            }
            catch (Exception ex)
            {

                Log.Error(ex, "Error occurs while executing (SP_GetAllLDLAppWithDetails)");

            }
            return ldlapplist;
        }

        public static async Task <string> GetLicenseTypeUsingLDLAppID(int LDLAppID)
        {
            string LicenseType = "";
            try {
                using (SqlConnection connection = new SqlConnection(Settings.ConnectionString))
                {
                    using (SqlCommand Command = new SqlCommand("SP_GetLicenseTypeByLDLAppID", connection))
                    {
                        Command.CommandType = CommandType.StoredProcedure;

                        Command.Parameters.AddWithValue("@LDLAppID", LDLAppID);

                        SqlParameter outputparam = new SqlParameter("@LicenseType", DbType.String)
                        {
                            Direction = ParameterDirection.Output,
                        };

                        Command.Parameters.Add(outputparam);

                        connection.Open();
                        using (SqlDataReader Reader = await Command.ExecuteReaderAsync())
                        {
                            if (Reader.Read())
                            {
                                LicenseType = (string)outputparam.Value;


                            }
                        }
                    }
                }
            }catch (Exception ex)
            {
                LicenseType = "";
                Settings.AddErrorToEventViewer("Error in Get LicenstypeByLDLAppID ",ex.Message);
            }
            
            return LicenseType;
        }

        public static bool FindLDLApp(int LDLAppID,ref int AppID,ref int LicenseTypeID)
        {
            Log.Information($"Starting FindLDLApp in LDLAppDataLayer with LDLAppID : {LDLAppID}");

            bool IsFound = false;
            try
            {
                using (SqlConnection Connection = new SqlConnection(Settings.ConnectionString))
                {
                    Connection.Open();
                    Log.Information("Connection To database established successfully");

                    using (SqlCommand Command = new SqlCommand("SP_GetLDLAppByLDLAppID", Connection))
                    {
                        Command.CommandType = CommandType.StoredProcedure;

                        Command.Parameters.AddWithValue("@LDLAppID", LDLAppID);

                        using (SqlDataReader Reader = Command.ExecuteReader())
                        {
                            if (Reader.Read())
                            {
                                Log.Information("Reader is Read");
                                IsFound = true;
                                AppID = (int)Reader["ApplicationID"];
                                LicenseTypeID = (int)Reader["LicenseClassID"];
                            }
                        }
                    }
                }

                Log.Information("FindLDLApp (LDLAppDataLAyer)  executed successfully");

            }catch (SqlException ex)
            {
                Log.Error(ex, "Error occured while fetching data from database");
                IsFound = false;
            }
           
            return IsFound;
        }

        public static async Task<bool>IsLDLAppExists(int LDLAppID)
        {
            Log.Information("Start execution IsLDLAppExists (LDLAppDataLayer)");
            bool IsFound = false;

            try
            {
                using (SqlConnection Connection=new SqlConnection(Settings.ConnectionString))
                {
                    await Connection.OpenAsync();
                    Log.Information("Connection to database established successfully");

                    using (SqlCommand Command=new SqlCommand("SP_IsLDLAppExists", Connection))
                    {
                        Command.CommandType = CommandType.StoredProcedure;

                        Command.Parameters.AddWithValue("@LDLAppID", LDLAppID);

                        using (SqlDataReader Reader = await Command.ExecuteReaderAsync())
                        {
                            IsFound = Reader.HasRows;
                        }
                    }

                    Log.Information($"IsFound = {IsFound}");
                }
            }catch(SqlException ex)
            {
                Log.Error(ex, "Unexpected error occured during fetching data from database", ex.Message);
                IsFound = false;
            }
            return IsFound;
        }

        public static async Task<int>GetNumberOfPassedTestsForLDLApp(int LDLAppID)
        {
            Log.Information("Start GetNumberOfPassedTestsForLDLApp in LDLAppDataLayer");
            
            int NumberOfPassedTests = -1;
            try
            {
                using (SqlConnection Connection = new SqlConnection(Settings.ConnectionString))
                {
                    await Connection.OpenAsync();
                    Log.Information("Connection To database established successfully");

                    using (SqlCommand Command = new SqlCommand("SP_GetNumberOfTestsPassedForLDLAppID", Connection))
                    {
                        Command.CommandType = CommandType.StoredProcedure;

                        Command.Parameters.AddWithValue("@LDLAppID", LDLAppID);

                        SqlParameter outputparam = new SqlParameter("@NumberOfPassedTests", DbType.Int32)
                        {
                            Direction = ParameterDirection.Output,
                        };
                        Command.Parameters.Add(outputparam);

                        object result =await Command.ExecuteScalarAsync();
                        if (result != null && int.TryParse(result.ToString(), out int numofpasstests))
                        {
                            NumberOfPassedTests = numofpasstests;
                        }
                    }
                }

                Log.Information("GetNumberOfPassedTestsForLDLApp in LDLAppDataLayer executed successfully");
            }
            catch (SqlException ex)
            {
                Log.Error(ex, "Unexpected error occurs ", ex.Message);
                NumberOfPassedTests = -1;
            }

            return NumberOfPassedTests;

        }
         

    }
}
