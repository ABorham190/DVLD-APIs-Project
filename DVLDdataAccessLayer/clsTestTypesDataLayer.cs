using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Runtime.CompilerServices;

namespace DVLDdataAccessLayer
{
    public class clsTestTypesDataLayer
    {

        public static DataTable GetExaminationsList()
        {
            DataTable table = new DataTable();
            string Querey = @"Select * from TestTypes;";
            SqlConnection Connectoin = new SqlConnection(Settings.ConnectionString);
            SqlCommand Command = new SqlCommand(Querey, Connectoin);

            try
            {
                Connectoin.Open();
                SqlDataReader Reader = Command.ExecuteReader();
                if (Reader.HasRows)
                {
                    table.Load(Reader);
                }
                Reader.Close();
            }catch (Exception ex)
            {

            }
            finally
            {
                Connectoin.Close();
            }

            return table;
        }

        public static bool FindTestByID(int TestID,ref string Title,
            ref string Describtion,ref Decimal Fees)
        {
            bool IsFound = false;
            string Querey = @"Select * from TestTypes 
                           where TestTypeID=@TestID;";
            SqlConnection Connection=new SqlConnection(Settings.ConnectionString);
            SqlCommand Command=new SqlCommand(Querey, Connection);
            Command.Parameters.AddWithValue("@TestID", TestID);

            try
            {
                Connection.Open();
                SqlDataReader Reader = Command.ExecuteReader();
                if (Reader.Read())
                {
                    IsFound = true;
                    Title = (string)Reader["TestTypeTitle"];
                    Describtion = (string)Reader["TestTypeDescription"];

                    Fees = (Decimal)Reader["TestTypeFees"];
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

        public static bool UpdateTestDetails(int TestID,string Title,
            string Describtion ,Decimal Fees)
        {
            int NumberOfAffectedRows = 0;
            string Querey = @"Update ExaminationDetails 
                            set ExaminationName=@Title,Details=@Describtion,
                            ExaminationFees=@Fees
                            where ID=@TestID;";

            SqlConnection Connection=new SqlConnection(Settings.ConnectionString);
            SqlCommand Command=new SqlCommand( Querey, Connection);
            Command.Parameters.AddWithValue("@TestID", TestID);
            Command.Parameters.AddWithValue("@Title", Title);
            Command.Parameters.AddWithValue("@Describtion", Describtion);
            Command.Parameters.AddWithValue("@Fees", Fees);

            try
            {
                Connection.Open();
                NumberOfAffectedRows = Command.ExecuteNonQuery();

            }catch(Exception ex)
            {
                NumberOfAffectedRows = 0;
            }
            finally
            {
                Connection.Close();
            }
            return NumberOfAffectedRows > 0;
        }

        public static int GetNumberOfVisionTestFails(int DLAppID)
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
                             and TestAppointments.TestTypeID=1;
                             ;";

            SqlConnection Connection=   new SqlConnection(Settings.ConnectionString);
            SqlCommand Command=new SqlCommand(Querey, Connection);
            Command.Parameters.AddWithValue("@DLAppID", DLAppID);

            try
            {
                Connection.Open();
                Object Result = Command.ExecuteScalar();
                if(Result!=null&&int.TryParse(Result.ToString(),out int Num))
                {
                    Trials = Num;
                }
            }catch(Exception ex)
            {
                Trials=0;
            }
            finally
            {
                Connection.Close ();
            }

            return Trials;
        }

        public static int GetPassedTests(int DLAppID)
        {
            int passedTests = 0;
            string Querey = @"select Count(TestID)
                           from Tests inner join TestAppointments on 
                           Tests.TestAppointmentID=TestAppointments.TestAppointmentID
                           inner join LocalDrivingLicenseApplications
                           on TestAppointments.LocalDrivingLicenseApplicationID=
                           LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID
                           where LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID=@DLAppID
                           and Tests.TestResult=1 ;";

            SqlConnection Connection= new SqlConnection(Settings.ConnectionString); 
            SqlCommand Command=new SqlCommand( Querey, Connection);

            Command.Parameters.AddWithValue("@DLAppID", DLAppID);
            try
            {
                Connection.Open();
                Object Result = Command.ExecuteScalar();
                if (Result!=null && int.TryParse(Result.ToString(),out int PTests))
                {
                    passedTests = PTests;
                }
                
            }catch (Exception ex)
            {
                passedTests = 0;
            }
            finally { Connection.Close ();}
            return passedTests;
        }

        public static Decimal GetTestFees(int TestTypeID)
        {
            Decimal TestFees = 0;
            string Querey = @"select TestTypes.TestTypeFees from 
                           TestTypes where TestTypes.TestTypeID=@TestTypeID;
                           ";
            SqlConnection Connection = new SqlConnection(Settings.ConnectionString);
            SqlCommand Command=new SqlCommand(Querey, Connection);
            Command.Parameters.AddWithValue("@TestTypeID", TestTypeID);
            try
            {
                Connection.Open();
                SqlDataReader Reader = Command.ExecuteReader();
                if (Reader.Read())
                {
                    TestFees = (Decimal)Reader["TestTypeFees"];
                   
                }
                Reader.Close();
            }
            catch(Exception ex)
            {
                TestFees = 0;
            }
            finally
            {
                Connection.Close();
            }
            return TestFees;
        }

        




    }
}
