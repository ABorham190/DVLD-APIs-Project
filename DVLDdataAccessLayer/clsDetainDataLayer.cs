using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Microsoft.Data.SqlClient;
using Serilog;
using System.ComponentModel;

namespace DVLDdataAccessLayer
{
    public  class clsDetainDataLayer
    {
        public class GetDetianedLicenseDTO
        {
            public int DetainID { get; set; }
            public int LicenseID { get; set; }
            public DateTime DetainDate { get; set; }
            public bool IsReleased { get; set; }
            public Decimal FineFees { get; set; }
            public string NationalNo { get; set; }
            public string Name { get; set; }
            public DateTime? ReleaseDate { get; set; }
            public int? ReleaseApplicationID { get; set; }

        }
        public static async Task<int> AddNewDetain(int LicenseID,DateTime DetainDate,
            Decimal FineFees,int CreatedByUserID,bool IsRelease
            )
        {
            Log.Information("Start Executing AddNewDetain clsDetainDataLayer");
            int InsertedID = 0;
            try { 
                 string Querey = @"Insert into DetainedLicenses (LicenseID,DetainDate,
                              FineFees,CreatedByUserID,IsReleased)  values (@LicenseID,@DetainDate,
                              @FineFees,@CreatedByUserID,@IsReleased);
                              select scope_identity();";

                using (SqlConnection connection = new SqlConnection(Settings.ConnectionString))
                {
                    using (SqlCommand Command = new SqlCommand(Querey, connection))
                    {
                        Command.Parameters.AddWithValue("@LicenseID", LicenseID);
                        Command.Parameters.AddWithValue("@DetainDate", DetainDate);
                        Command.Parameters.AddWithValue("@FineFees", FineFees);
                        Command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);
                        Command.Parameters.AddWithValue("@IsReleased", IsRelease);



                        await connection.OpenAsync();
                        Log.Information("Connection to databse established successfully");
                        object Result =await  Command.ExecuteScalarAsync();

                        if (Result != null && int.TryParse(Result.ToString(), out int ID))
                        {
                            InsertedID = ID;
                        }
                    }
                }

                Log.Information("AddNewDetain clsDetainDataLayer Executed successfully");
            }
            catch(Exception ex)
            {
                Log.Error(ex, "Unexepected Error ", ex.Message);
            }

            return InsertedID ;

        }

        public static bool FindDetainByLicenseID(int LicenseID,ref int DetainID,
            ref DateTime DetainDate,ref Decimal FineFees,
            ref int CreatedByUserID)
        {
            bool IsFound = false;
            string Querey = @"select DetainID,DetainDate,FineFees,CreatedByUserID 
                             from DetainedLicenses
                             where LicenseID= @LicenseID and IsReleased=0;";
            SqlConnection Connection= new SqlConnection(Settings.ConnectionString);
            SqlCommand Command = new SqlCommand(Querey, Connection);
            Command.Parameters.AddWithValue("@LicenseID", LicenseID);
            try
            {
                Connection.Open();
                SqlDataReader Reader = Command.ExecuteReader();
                if (Reader.Read())
                {
                    IsFound = true;
                    DetainID = (int)Reader["DetainID"];
                    DetainDate = (DateTime)Reader["DetainDate"];
                    FineFees = (Decimal)Reader["FineFees"];
                    CreatedByUserID = (int)Reader["CreatedByUserID"];
                    
                }
                Reader.Close();
            }
            catch (Exception ex)
            {
            }
            finally
            {
                Connection.Close();
            }
            return IsFound ;
        }
        public static bool UpdateDetain(int DetainID,bool IsReleased,
            DateTime ReleaseDate,int ReleaseByUserID,
            int ReleaseApplicationID)
        {
            int NumberOfAffectedRows = 0;
            string Querey = @"Update DetainedLicenses Set IsReleased = @IsReleased,
                              ReleaseDate=@ReleaseDate,ReleasedByUserID=@ReleasedByUserID,
                              ReleaseApplicationID=@ReleaseApplicationID
                              where DetainID=@DetainID;";

            SqlConnection Connection = new SqlConnection(Settings.ConnectionString);
            SqlCommand Command = new SqlCommand(Querey, Connection);
            Command.Parameters.AddWithValue("@IsReleased", IsReleased);
            Command.Parameters.AddWithValue("@ReleaseDate", ReleaseDate);
            Command.Parameters.AddWithValue("@ReleasedByUserID", ReleaseByUserID);
            Command.Parameters.AddWithValue("@ReleaseApplicationID", ReleaseApplicationID);
            Command.Parameters.AddWithValue("@DetainID", DetainID);


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

        public static async Task<int>ReleaseDetainedLicense(int LicenseID,int ReleaseApplicationID,int ReleasedByUserID)
        {
            int NumberOfAffectedRows = 0;
            try
            {
                using (SqlConnection Connection = new SqlConnection(Settings.ConnectionString))
                {
                    using (SqlCommand Command = new SqlCommand("SP_ReleaseDetainedLicense", Connection))
                    {
                        Command.CommandType = CommandType.StoredProcedure;

                        Command.Parameters.AddWithValue("@LicenseID", LicenseID);
                        Command.Parameters.AddWithValue("@ReleasedByUserID", ReleasedByUserID);
                        Command.Parameters.AddWithValue("@ReleaseApplicationID", ReleaseApplicationID);

                        await Connection.OpenAsync();

                        NumberOfAffectedRows = await Command.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                NumberOfAffectedRows = 0;
            }

            return NumberOfAffectedRows;
        }
        public static async Task<List<GetDetianedLicenseDTO>> GetAllDetainedLicenses()
        {
            Log.Information("Start Executing GetAllDetainedLicenses func in clsDetainedDatalayer");
            List<GetDetianedLicenseDTO>detainedLicenses=new List<GetDetianedLicenseDTO> ();
            try {
                using (SqlConnection Connection = new SqlConnection(Settings.ConnectionString))
                {
                    using (SqlCommand Command = new SqlCommand("SP_GetAllDetainedLicenses", Connection))
                    {
                        Command.CommandType = CommandType.StoredProcedure;

                        await Connection.OpenAsync();
                        Log.Information("Connection with database established successfully");

                        using (SqlDataReader Reader = await Command.ExecuteReaderAsync())
                        {
                            while (await Reader.ReadAsync())
                            {
                                var detainedlicenseDTO = new GetDetianedLicenseDTO();

                                detainedlicenseDTO.DetainID = (int)Reader["DetainID"];
                                detainedlicenseDTO.LicenseID = (int)Reader["LicenseID"];
                                detainedlicenseDTO.DetainDate = (DateTime)Reader["DetainDate"];
                                detainedlicenseDTO.FineFees = (Decimal)Reader["FineFees"];
                                detainedlicenseDTO.Name = (string)Reader["Name"];
                                detainedlicenseDTO.NationalNo = (string)Reader["NationalNo"];
                                detainedlicenseDTO.IsReleased = (bool)Reader["IsReleased"];

                                if (Reader["ReleaseApplicationID"] != DBNull.Value)
                                    detainedlicenseDTO.ReleaseApplicationID = (int)Reader["ReleaseApplicationID"];

                                if (Reader["ReleaseDate"] != DBNull.Value)
                                {
                                    detainedlicenseDTO.ReleaseDate = (DateTime)Reader["ReleaseDate"];
                                }

                                detainedLicenses.Add(detainedlicenseDTO);
                            }
                        }
                    }
                }
                
            }catch(Exception ex)
            {
                Log.Error(ex,"unexcepected Error in Sql",ex.Message);
            }
            return detainedLicenses;
        }
    }
}
