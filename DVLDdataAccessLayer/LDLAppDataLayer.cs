using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace DVLDdataAccessLayer
{
    public class LDLAppDataLayer
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
            int InsertedID = -1;
            try
            {
                
                using (SqlConnection Connection = new SqlConnection(Settings.ConnectionString))
                {

                    Connection.Open();

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
                            InsertedID = (int)outPutParam.Value;
                        }


                    }
                }
            }
            catch (Exception ex)
            {
                InsertedID = -1;
                Settings.AddErrorToEventViewer("Error In Add New LDLApp ", ex.Message);
            }
                
               
            
            return InsertedID;

        }

        public static async Task< List<LDLAppDTO>> GetAllLDLApps()
        {
            
            List<LDLAppDTO>ldlapplist=new List<LDLAppDTO>();
            try
            {
                using (SqlConnection Connection = new SqlConnection(Settings.ConnectionString))
                {

                    using (SqlCommand Command = new SqlCommand("SP_GetAllLDLAppWithDetails", Connection))
                    {
                        Command.CommandType = CommandType.StoredProcedure;

                        Connection.Open();
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
            }
            catch (Exception ex)
            {

                Settings.AddErrorToEventViewer("Error In Get All LDLAppa", ex.Message);

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
            bool IsFound = false;
            try
            {
                using (SqlConnection Connection = new SqlConnection(Settings.ConnectionString))
                {
                    using (SqlCommand Command = new SqlCommand("SP_GetLDLAppByLDLAppID", Connection))
                    {
                        Command.CommandType = CommandType.StoredProcedure;

                        Command.Parameters.AddWithValue("@LDLAppID", LDLAppID);



                        Connection.Open();
                        using (SqlDataReader Reader = Command.ExecuteReader())
                        {
                            if (Reader.Read())
                            {
                                IsFound = true;
                                AppID = (int)Reader["ApplicationID"];
                                LicenseTypeID = (int)Reader["LicenseClassID"];
                            }
                        }
                    }
                }

            }catch (Exception ex)
            {
                IsFound = false;
            }
           
            return IsFound;
        }

         

    }
}
