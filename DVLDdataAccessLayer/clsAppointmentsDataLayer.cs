using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GlobalStructs;

namespace DVLDdataAccessLayer
{
    public class clsAppointmentsDataLayer
    {
        public static int AddNewAppointment(int TestTypeID,int DLAppID,
            DateTime AppointmentDate,Decimal PaidFees,int CreatedByUserID,
            bool IsLocked,int RetakeTestAppID)
        {
            int InsertedID = -1;
            string Querey = @"insert into TestAppointments 
                            (TestTypeID,LocalDrivingLicenseApplicationID,
                            AppointmentDate,PaidFees,CreatedByUserID
                            ,IsLocked,RetakeTestApplicationID) values 
                            (@TestTypeID,@DLAppID,@AppointmentDate,@PaidFees,
                            @CreatedByUserID,@IsLocked,@RetakeTestAppID);
                             select scope_identity();";

            SqlConnection Connection =new SqlConnection(Settings.ConnectionString);
            SqlCommand Command = new SqlCommand(Querey, Connection);

            Command.Parameters.AddWithValue("@TestTypeID", TestTypeID);
            Command.Parameters.AddWithValue("@DLAppID",DLAppID);
            Command.Parameters.AddWithValue("@AppointmentDate",AppointmentDate);
            Command.Parameters.AddWithValue("@PaidFees",PaidFees);
            Command.Parameters.AddWithValue("@CreatedByUserID",CreatedByUserID);
            Command.Parameters.AddWithValue("@IsLocked",IsLocked);


            if (RetakeTestAppID != 0)
                Command.Parameters.AddWithValue("@RetakeTestAppID", RetakeTestAppID);
            else
                Command.Parameters.AddWithValue("@RetakeTestAppID", DBNull.Value);


            try
            {
                Connection.Open();
                Object Result = Command.ExecuteScalar();
                if (Result !=null&&int.TryParse(Result.ToString(),out int ID))
                {
                    InsertedID = ID;
                }
            }catch (Exception ex)
            {
                InsertedID=-1;
            }
            finally
            {
                Connection.Close();
            }
            return InsertedID ;
        }

        public static DataTable GetAllVisionTestAppointmentsforDLAppID(int DLAppID)
        {
            DataTable dt = new DataTable();
            string Querey = @"select TestAppointmentID as AppointmentID ,
                              AppointmentDate ,PaidFees, IsLocked from 
                              TestAppointments
                              where 
                               TestAppointments.TestTypeID=1 and 
                              TestAppointments.LocalDrivingLicenseApplicationID=@DLAppID
                              Order by AppointmentID desc;";
            SqlConnection connection = new SqlConnection(Settings.ConnectionString);
            SqlCommand Command = new SqlCommand( Querey, connection);
            Command.Parameters.AddWithValue("@DLAppID", DLAppID);

            try
            {
                connection.Open();
                SqlDataReader Reader = Command.ExecuteReader();
                if (Reader.HasRows)
                {
                    dt.Load(Reader);
                }
                Reader.Close();
            }catch (Exception ex)
            {

            }
            finally
            {
                connection.Close();
            }
            return dt;
        }
        public static bool IsThisDLAppIDHasAnyAppointments(int DLAppID)
        {
            bool IsFound=false;
            string Querey = @"select top 1 found=1 from TestAppointments
                              where LocalDrivingLicenseApplicationID=@DLAppID 
                              and TestAppointments.TestTypeID=1;";
            SqlConnection Connection = new SqlConnection(Settings.ConnectionString);
            SqlCommand Command=new SqlCommand( Querey, Connection);

            Command.Parameters.AddWithValue("@DLAppID",DLAppID);

            try
            {
                Connection.Open();
                SqlDataReader Reader = Command.ExecuteReader();
                if (Reader.HasRows)
                {
                    IsFound = true;

                }
                Reader.Close();
            }catch(Exception ex)
            {
                IsFound = false;
            }
            finally
            {
                Connection.Close();

            }
            return IsFound;
        }

        public static bool IsThisDLAppIDHasAnyActiveVissionTestAppointment(int DLAppID)
        {
            bool IsFound=false;
            string Querey = @"select Found=1 from TestAppointments
                             where LocalDrivingLicenseApplicationID
                             = @DLAppID and TestAppointments.TestTypeID=1 and 
                             TestAppointments.IsLocked= 0
                             ;
                             ";

            SqlConnection Connection=new SqlConnection(Settings.ConnectionString);
            SqlCommand Command=new SqlCommand(Querey, Connection);
            Command.Parameters.AddWithValue("@DLAppID", DLAppID);

            try
            {
                Connection.Open();
                SqlDataReader Reader = Command.ExecuteReader();
                if (Reader.HasRows)
                {
                    IsFound = true;
                }
                Reader.Close();
            }
            catch(Exception ex)
            {
                IsFound=false;
            }
            finally
            {
                Connection.Close();
            }
            return IsFound;
        }

        public static bool UpdateAppointment(int AppointmentID,bool IsAppointmentLocked ,
            DateTime AppointmentDate)
        {
            int NumberOfAffectedRows = 0;
            string Querey = @"Update TestAppointments  set 
                            AppointmentDate=@AppointmentDate
                            ,IsLocked=@IsAppointmentlocked 
                            where TestAppointmentID=@AppointmentID;";

            SqlConnection Connection = new SqlConnection(Settings.ConnectionString);    
            SqlCommand Command=new SqlCommand( Querey, Connection);
            Command.Parameters.AddWithValue("@AppointmentID", AppointmentID);
            
            Command.Parameters.AddWithValue("@IsAppointmentlocked", IsAppointmentLocked);
            Command.Parameters.AddWithValue("@AppointmentDate", AppointmentDate);

            try
            {
                Connection.Open();
                NumberOfAffectedRows = Command.ExecuteNonQuery();


            }catch(Exception ex)
            {
                NumberOfAffectedRows=0;
            }
            finally
            {
                Connection.Close();
            }
            return NumberOfAffectedRows > 0;
        }

        public static bool FindAppointmentApp(int AppointmentID
            ,ref int TestTypeID,ref int LocalDrivingLicenseAppID,
            ref DateTime AppointmentDate,ref Decimal PaidFees,
            ref int CreatedByUserID,ref bool IsAppointmentLocked,
            ref int RetakeTestApplicationID)
        {
            bool IsFound = false;
            string Querey = @"select * from TestAppointments where
                              TestAppointments.TestAppointmentID=@AppointmentID;
                              ;";

            SqlConnection Connection =new SqlConnection(Settings.ConnectionString); 
            SqlCommand Command=new SqlCommand(Querey, Connection);

            Command.Parameters.AddWithValue("@AppointmentID", AppointmentID);

            try
            {
                Connection.Open();
                SqlDataReader Reader = Command.ExecuteReader();
                if (Reader.Read())
                {
                    IsFound = true;
                    TestTypeID = (int)Reader["TestTypeID"];
                    LocalDrivingLicenseAppID = (int)Reader["LocalDrivingLicenseApplicationID"];
                    AppointmentDate = (DateTime)Reader["AppointmentDate"];
                    PaidFees = (Decimal)Reader["PaidFees"];
                    CreatedByUserID = (int)Reader["CreatedByUserID"];
                    IsAppointmentLocked = (bool)Reader["IsLocked"];
                    if (Reader["RetakeTestApplicationID"] == DBNull.Value)
                    {
                        RetakeTestApplicationID = -1;
                    }
                    else
                    {
                        RetakeTestApplicationID = (int)Reader["RetakeTestApplicationID"];
                    }

                }
                Reader.Close();
            }catch (Exception ex)
            {
                IsFound=false;
            }
            finally 
            { 
                Connection.Close(); 
            }
            return IsFound;
        }

        public static bool GetTakeTestDetails(int AppointmentID,ref GeneralStructs TakeTestStruct)
        {
            bool IsFound = false;
            string Querey = @"select TestAppointments.LocalDrivingLicenseApplicationID,
                             Applications.ApplicationID,
                             LicenseClasses.ClassName,(People.FirstName+' '+People.SecondName
                             +' '+People.ThirdName+' '+People.LastName)as Name  
                              ,TestAppointments.AppointmentDate,
                             TestTypes.TestTypeFees ,TestAppointments.RetakeTestApplicationID
                             from TestAppointments 
                              inner join LocalDrivingLicenseApplications
                             on TestAppointments.LocalDrivingLicenseApplicationID=
                             LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID
                             inner join LicenseClasses on LocalDrivingLicenseApplications.LicenseClassID=
                             LicenseClasses.LicenseClassID inner join Applications on
                             LocalDrivingLicenseApplications.ApplicationID=
                             Applications.ApplicationID inner join 
                             People on Applications.ApplicantPersonID=People.PersonID 
                             inner join TestTypes on TestAppointments.TestTypeID=
                             TestTypes.TestTypeID
                             
                             where TestAppointments.TestAppointmentID=@AppointmentID;
                             ";
            SqlConnection connection =new SqlConnection(Settings.ConnectionString);
            SqlCommand Command=new SqlCommand( Querey, connection);
            Command.Parameters.AddWithValue("@AppointmentID", AppointmentID);

            try
            {
                connection.Open();
                SqlDataReader Reader = Command.ExecuteReader();
                if (Reader.Read())
                {
                    IsFound = true;
                    TakeTestStruct.ClassName = (string)Reader["ClassName"];
                    TakeTestStruct.Date = (DateTime)Reader["AppointmentDate"];
                    TakeTestStruct.DLAppID = (int)Reader["LocalDrivingLicenseApplicationID"];
                    TakeTestStruct.ApplicantName = (string)Reader["Name"];
                    TakeTestStruct.AppointmentID = AppointmentID;
                    
                    TakeTestStruct.Fees = (Decimal)Reader["TestTypeFees"];
                    TakeTestStruct.ApplicationID = (int)Reader["ApplicationID"];
                    if (Reader["RetakeTestApplicationID"]==DBNull.Value)
                    {
                        TakeTestStruct.RetakeTestAppID = -1;
                    }
                    else
                    {
                        TakeTestStruct.RetakeTestAppID = (int)Reader["RetakeTestApplicationID"];
                    }



                }
                Reader.Close();
            }
            catch (Exception ex)
            {
                IsFound = false;
            }
            finally
            {
                connection.Close();
            }

            return IsFound;
        }

        public static bool IsThisDLAppIDPassVisionTest(int DLAppID)
        {
            bool IsFound = false;
            string Querey = @"select found=1 from 
                             Tests inner join TestAppointments on
                             Tests.TestAppointmentID=
                             TestAppointments.TestAppointmentID
                             inner join LocalDrivingLicenseApplications
                             on TestAppointments.LocalDrivingLicenseApplicationID=
                             LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID
                             where TestAppointments.LocalDrivingLicenseApplicationID=@DLAppID 
                             and TestAppointments.TestTypeID=1 and Tests.TestResult=1;
                             ";

            SqlConnection Connection= new SqlConnection(Settings.ConnectionString); 
            SqlCommand Command=new SqlCommand(Querey, Connection);

            Command.Parameters.AddWithValue("@DLAppID", DLAppID);
            try
            {
                Connection.Open();
                SqlDataReader Reader = Command.ExecuteReader();
                IsFound = Reader.HasRows;
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

        public static bool IsThereAreAnyVisionAppointmentsForThisDLAppID(int DLAppID)
        {
            bool IsFound = false;
            string Querey = @"select top 1  found=1 from TestAppointments
                             where LocalDrivingLicenseApplicationID=@DLAppID and 
                             TestAppointments.TestTypeID=1;
                             ";
            SqlConnection Connection=new SqlConnection(Settings.ConnectionString);
            SqlCommand Command=new SqlCommand( Querey, Connection);
            Command.Parameters.AddWithValue("@DLAppID", DLAppID);

            try
            {
                Connection.Open();
                SqlDataReader Reader=Command.ExecuteReader();
                IsFound= Reader.HasRows;
                Reader.Close();
            }catch (Exception ex)
            {
                IsFound= false;
            }
            finally
            {
                Connection.Close();
            }

            return IsFound;
        }


        public static bool LockAnAppointment(int AppointmentID)
        {
            int NumberOfAffectedRows = 0;
            string Querey = @"Update TestAppointments 
                             set IsLocked=1 
                             where TestAppointmentID=@TestAppointmentID";

            SqlConnection Connection= new SqlConnection(Settings.ConnectionString);
            SqlCommand Command=new SqlCommand(Querey, Connection);
            Command.Parameters.AddWithValue("@TestAppointmentID", AppointmentID);
            try
            {
                Connection.Open();
                NumberOfAffectedRows = Command.ExecuteNonQuery();

            }catch (Exception ex)
            {
                NumberOfAffectedRows=0;
            }
            finally
            {
                Connection.Close();
            }
            return NumberOfAffectedRows > 0;
        }

        public static DataTable GetAllWrittenTestAppointmentsForThisLDLAppID(int LDLAppID)
        {
            DataTable dt = new DataTable();
            string Querey = @"select TestAppointmentID as AppointmentID ,
                              AppointmentDate ,PaidFees, IsLocked from 
                              TestAppointments
                              where 
                               TestAppointments.TestTypeID=2 and 
                              TestAppointments.LocalDrivingLicenseApplicationID=@DLAppID
                              Order by AppointmentID desc;";
            SqlConnection connection = new SqlConnection(Settings.ConnectionString);
            SqlCommand Command = new SqlCommand(Querey, connection);
            Command.Parameters.AddWithValue("@DLAppID", LDLAppID);

            try
            {
                connection.Open();
                SqlDataReader Reader = Command.ExecuteReader();
                if (Reader.HasRows)
                {
                    dt.Load(Reader);
                }
                Reader.Close();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                connection.Close();
            }
            return dt;
        }
        public static DataTable GetAllStreetTestAppointments(int LDLAppID)
        {
            DataTable dt = new DataTable();
            string Querey = @"select TestAppointmentID as AppointmentID ,
                              AppointmentDate ,PaidFees, IsLocked from 
                              TestAppointments
                              where 
                               TestAppointments.TestTypeID=3 and 
                              TestAppointments.LocalDrivingLicenseApplicationID=@DLAppID
                              Order by AppointmentID desc;";
            SqlConnection connection = new SqlConnection(Settings.ConnectionString);
            SqlCommand Command = new SqlCommand(Querey, connection);
            Command.Parameters.AddWithValue("@DLAppID", LDLAppID);

            try
            {
                connection.Open();
                SqlDataReader Reader = Command.ExecuteReader();
                if (Reader.HasRows)
                {
                    dt.Load(Reader);
                }
                Reader.Close();
            }
            catch (Exception ex)
            {

            }
            finally
            {
                connection.Close();
            }
            return dt;
        }
        public static bool IsThisLDLAppIDPassedTest(int LDLAppID,int TestTypeID)
        {
            bool IsFound = false;
            string Querey = @"select found=1 from 
                             Tests inner join TestAppointments on
                             Tests.TestAppointmentID=
                             TestAppointments.TestAppointmentID
                             inner join LocalDrivingLicenseApplications
                             on TestAppointments.LocalDrivingLicenseApplicationID=
                             LocalDrivingLicenseApplications.LocalDrivingLicenseApplicationID
                             where TestAppointments.LocalDrivingLicenseApplicationID=@DLAppID 
                             and TestAppointments.TestTypeID=@TestTypeID and Tests.TestResult=1;
                             ";

            SqlConnection Connection = new SqlConnection(Settings.ConnectionString);
            SqlCommand Command = new SqlCommand(Querey, Connection);

            Command.Parameters.AddWithValue("@DLAppID", LDLAppID);
            Command.Parameters.AddWithValue("@TestTypeID", TestTypeID);
            try
            {
                Connection.Open();
                SqlDataReader Reader = Command.ExecuteReader();
                IsFound = Reader.HasRows;
                Reader.Close();
            }
            catch (Exception ex)
            {
                IsFound = false;
            }
            finally
            {
                Connection.Close();
            }
            return IsFound;
        }

        public static bool IsThisLDLAppIDHasAnyActiveAppointments(int LDLAppID,int TestTypeID,ref int AppointmentID)
        {
            bool IsFound = false;
            string Querey = @"select Found=1 , TestAppointmentID 
                              from TestAppointments
                             where LocalDrivingLicenseApplicationID
                             = @LDLAppID and TestAppointments.TestTypeID=@TestTypeID and
                             TestAppointments.IsLocked= 0
                             ;
                             ";

            SqlConnection Connection = new SqlConnection(Settings.ConnectionString);
            SqlCommand Command = new SqlCommand(Querey, Connection);
            Command.Parameters.AddWithValue("@LDLAppID", LDLAppID);
            Command.Parameters.AddWithValue("@TestTypeID", TestTypeID);

            try
            {
                Connection.Open();
                SqlDataReader Reader = Command.ExecuteReader();
                if (Reader.Read())
                {
                    IsFound = true;
                    AppointmentID = (int)Reader["TestAppointmentID"];
                }
                Reader.Close();
            }
            catch (Exception ex)
            {
                IsFound = false;
            }
            finally
            {
                Connection.Close();
            }
            return IsFound;
        }

        public static bool IsThereAreAnyAppointmentsForThisLDLAppID(int LDLAppID, int TestTypeID)
        {
            bool IsFound = false;
            string Querey = @"select top 1  found=1 from TestAppointments
                             where LocalDrivingLicenseApplicationID=@DLAppID and 
                             TestAppointments.TestTypeID=@TestTypeID ;
                             ";
            SqlConnection Connection = new SqlConnection(Settings.ConnectionString);
            SqlCommand Command = new SqlCommand(Querey, Connection);
            Command.Parameters.AddWithValue("@DLAppID", LDLAppID);
            Command.Parameters.AddWithValue("@TestTypeID", TestTypeID);

            try
            {
                Connection.Open();
                SqlDataReader Reader = Command.ExecuteReader();
                IsFound = Reader.HasRows;
                Reader.Close();
            }
            catch (Exception ex)
            {
                IsFound = false;
            }
            finally
            {
                Connection.Close();
            }

            return IsFound;
        }

        //public static bool GetScheduleTestInfo(int TestAppointmentID, ScheduleTestInfo info)
        //{

        //}
        

        }

    }


