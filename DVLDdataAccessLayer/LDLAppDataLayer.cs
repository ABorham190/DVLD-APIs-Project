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

        public static int AddNewLDLApp(int AppID,int LicenseClassID)
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
                        if ((NumberOfAffectedRow = Command.ExecuteNonQuery()) > 0)
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

        public static DataTable GetAllLDLApps()
        {
            DataTable dt = new DataTable();
            using (SqlConnection Connection = new SqlConnection(Settings.ConnectionString)) {  

                string Querey = @"select LocalDrivingLicenseApplicationID As LDLAppID 
                           ,LicenseClasses.ClassName as LicenseClass,
                           People.NationalNo,(People.FirstName+' '
                           +People.SecondName+' '+People.ThirdName+' '+
                           People.LastName)as FullName,Applications.ApplicationDate,
                           (select count(TestAppointments.TestTypeID) from Tests
						   inner join TestAppointments on 
                           Tests.TestAppointmentID=TestAppointments.TestAppointmentID 
                            where TestAppointments.LocalDrivingLicenseApplicationID=
							LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID
							and TestResult=1) as PassedTests,Case When Applications.ApplicationStatus
							=1 Then 'New' when Applications.ApplicationStatus=2 then 'Cancelled'
							when Applications.ApplicationStatus=3 Then 'Completed' End As Status
                           from LocalDrivingLicenseApplications inner join
                           LicenseClasses on LocalDrivingLicenseApplications.LicenseClassID
                           =LicenseClasses.LicenseClassID inner join Applications on 
                           LocalDrivingLicenseApplications.ApplicationID
                           =Applications.ApplicationID inner join People on 
                            Applications.ApplicantPersonID=People.PersonID 
                           Order by LocalDrivingLicenseApplicationID desc;

                           ";


                using (SqlCommand Command = new SqlCommand(Querey, Connection))
                {

                    try
                    {
                        Connection.Open();
                        using (SqlDataReader Reader = Command.ExecuteReader())
                        {
                            if (Reader.HasRows)
                            {
                                dt.Load(Reader);
                            }
                        }
                    }
                    catch (Exception ex)
                    {

                        Settings.AddErrorToEventViewer("Error In Get All LDLAppa", ex.Message);

                    }
                }
            }

            return dt;
        }

        public static bool GetLicenseTypeUsingLDLAppID(int LDLAppID,ref string LicenseType)
        {
            LicenseType = "";
            string Querey = @"select LicenseTypes.LicenseType 
                           from NewLocalDrivingLicenseApplications inner join 
                           LicenseTypes
                           on NewLocalDrivingLicenseApplications.LicenseTypeID = 
                           LicenseTypes.LicenseTypeID
                           where 
                           NewLocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID=@LDLAppID;";

            SqlConnection connection=new SqlConnection(Settings.ConnectionString);
            SqlCommand Command=new SqlCommand(Querey, connection);
            Command.Parameters.AddWithValue("@LDLAppID", LDLAppID);

            try
            {
                connection.Open();
                SqlDataReader Reader = Command.ExecuteReader();
                if (Reader.Read()) 
                {
                    LicenseType = (string)Reader["LicenseType"];
                
                
                }
            }catch (Exception ex)
            {
                LicenseType = "";
            }
            finally
            {
                connection.Close();
            }
            return LicenseType != "";
        }

        public static bool FindLDLApp(int LDLAppID,ref int AppID,ref int LicenseTypeID)
        {
            bool IsFound = false;
            string Querey = @"select * from LocalDrivingLicenseApplications
                             where LocalDrivingLicenseApplicationID=@LDLAppID;";
            SqlConnection Connection = new SqlConnection(Settings.ConnectionString);
            SqlCommand Command=new SqlCommand(Querey,Connection);

            Command.Parameters.AddWithValue("@LDLAppID", LDLAppID);

            try
            {
                Connection.Open();
                SqlDataReader Reader = Command.ExecuteReader();
                if (Reader.Read())
                {
                    IsFound = true;
                    AppID = (int)Reader["ApplicationID"];
                    LicenseTypeID = (int)Reader["LicenseClassID"];
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

    }
}
