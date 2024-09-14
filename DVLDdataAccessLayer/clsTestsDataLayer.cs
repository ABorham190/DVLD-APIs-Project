using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DVLDdataAccessLayer
{
    public class clsTestsDataLayer
    {
        public static int AddNewTakenTest(int TestAppointmentID,
            bool TestResult,string Notes,int CreatedByUserID)
        {
            int InsertedID = 0;
            string Querey = @"Insert into Tests (TestAppointmentID,
                             TestResult,Notes,
                            CreatedByUserID) values (@TestAppointmentID,
                            @TestResult,@Notes,
                            @CreatedByUserID);
                            Select scope_identity();";

            SqlConnection Connection=new SqlConnection(Settings.ConnectionString);
            SqlCommand Command=new SqlCommand(Querey, Connection);

            Command.Parameters.AddWithValue("@TestAppointmentID", TestAppointmentID);
            Command.Parameters.AddWithValue("@TestResult",TestResult);
            if (Notes != "")
                Command.Parameters.AddWithValue("@Notes", Notes);
            else
                Command.Parameters.AddWithValue("@Notes", DBNull.Value);
            Command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);

            try
            {
                Connection.Open();
                Object Result = Command.ExecuteScalar();
                if (Result != null && int.TryParse(Result.ToString(), out int ID))
                {
                    InsertedID = ID;
                }


            }catch (Exception ex)
            {
                InsertedID = 0;
            }
            finally
            {
                Connection.Close();
            }

            return InsertedID ;

        }
        public static int GetNumberOfFailedTests(int LDLAppID,int TestTypeID)
        {
            int Trials = 0;
            string Querey = @"select count(TestID) from 
                             Tests inner join TestAppointments 
                             on Tests.TestAppointmentID=TestAppointments.TestAppointmentID
                             inner join LocalDrivingLicenseApplications on 
                             TestAppointments.LocalDrivingLicenseApplicationID=
                             LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID
                             where TestAppointments.LocalDrivingLicenseApplicationID=@DLAppID 
                             and Tests.TestResult=0
                             and TestAppointments.TestTypeID=@TestTypeID;
                             ;";

            SqlConnection Connection = new SqlConnection(Settings.ConnectionString);
            SqlCommand Command = new SqlCommand(Querey, Connection);
            Command.Parameters.AddWithValue("@DLAppID", LDLAppID);
            Command.Parameters.AddWithValue("@TestTypeID", TestTypeID);

            try
            {
                Connection.Open();
                Object Result = Command.ExecuteScalar();
                if (Result != null && int.TryParse(Result.ToString(), out int Num))
                {
                    Trials = Num;
                }
            }
            catch (Exception ex)
            {
                Trials = 0;
            }
            finally
            {
                Connection.Close();
            }

            return Trials;
        }
        

    }
}
