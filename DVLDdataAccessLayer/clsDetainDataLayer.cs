﻿using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using Microsoft.Data.SqlClient;
using Serilog;

namespace DVLDdataAccessLayer
{
    public  class clsDetainDataLayer
    {
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

        public static DataTable GetAllDetainedLicenses()
        {
            DataTable DetainedLicenses = new DataTable();
            string Querey = @"select DetainedLicenses.DetainID,
                             DetainedLicenses.LicenseID,
                             DetainedLicenses.DetainDate,
                             DetainedLicenses.IsReleased,
                             DetainedLicenses.FineFees,
                             DetainedLicenses.ReleaseDate,
                             People.NationalNo,
                             People.FirstName+' '+People.SecondName+
                             ' '+People.ThirdName+' '+People.LastName 
                             as Name ,DetainedLicenses.ReleaseApplicationID from 
                             DetainedLicenses inner join Licenses
                             on DetainedLicenses.LicenseID=
                             Licenses.LicenseID inner join 
                             Drivers on Licenses.DriverID=Drivers.DriverID
                             inner join People 
                             on Drivers.PersonID=People.PersonID;";
            SqlConnection Connection =new SqlConnection(Settings.ConnectionString);
            SqlCommand Command = new SqlCommand( Querey, Connection);

            try
            {
                Connection.Open();
                SqlDataReader Reader = Command.ExecuteReader();
                if (Reader.HasRows)
                {
                    DetainedLicenses.Load(Reader);
                }
                Reader.Close();
            }catch(Exception ex)
            {
            }
            finally
            {
                Connection.Close();
            }
            return DetainedLicenses;
        }
    }
}
