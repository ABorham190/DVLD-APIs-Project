using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Windows.Forms;

namespace DVLDdataAccessLayer
{
    public class clsOrdersDataLayer
    {

        public static DataTable GetAllOrders()
        {
            DataTable dt= new DataTable();
            string Querey = @"select OrderID ,LicenseTypes.LicenseType,NationalNumber,(FirstName+' '+SecondName+' '+ThirdName+' '+
                          LastName) as fullName , DateOfOrder,PassedTests,OrderCase from 
                          Orders inner join LicenseTypes on Orders.LicenseTypeID=LicenseTypes.LicenseTypeID
                          inner join Persons on Orders.PersonID=Persons.PersonID
                          inner join OrderCase on Orders.OrderCaseID=OrderCase.OrderCaseID;
                          ";
            SqlConnection Connection=new SqlConnection( Settings.ConnectionString );
            SqlCommand Command =new SqlCommand( Querey,Connection);
            try
            {
                Connection.Open();
                SqlDataReader Reader=Command.ExecuteReader();

                if (Reader.HasRows)
                {
                    dt.Load(Reader);
                }
                Reader.Close();
            }catch (Exception ex)
            {

            }finally { Connection.Close(); }
            return dt;

        }

        public static int AddNewOrder(int ApplicantPersonID,
            DateTime ApplicationDate,
            int ApplicationTypeID,byte ApplicationStatus,
            DateTime LastStatusDate , Decimal PaidFees,int CreatedByUserID)
        {
            int InsertedID = -1;
            string Querey = @"insert into Applications (ApplicantPersonID,
                            ApplicationDate,ApplicationTypeID,ApplicationStatus,
                             LastStatusDate,PaidFees,CreatedByUserID)
                            values (@ApplicantPersonID,
                            @ApplicationDate,@ApplicationTypeID,
                            @ApplicationStatus,
                            @LastStatusDate,@PaidFees,@CreatedByUserID);
                            select scope_identity();";
            SqlConnection Connection=new SqlConnection( Settings.ConnectionString );
            SqlCommand Command=new SqlCommand(Querey,Connection);
            Command.Parameters.AddWithValue("@ApplicantPersonID", ApplicantPersonID);
            Command.Parameters.AddWithValue("@ApplicationDate",ApplicationDate);
            Command.Parameters.AddWithValue("@ApplicationTypeID",ApplicationTypeID);
            Command.Parameters.AddWithValue("@ApplicationStatus", ApplicationStatus);
            Command.Parameters.AddWithValue("@LastStatusDate", LastStatusDate);
            Command.Parameters.AddWithValue("@PaidFees", PaidFees);
            Command.Parameters.AddWithValue("@CreatedByUserID", CreatedByUserID);

            try
            {
                Connection.Open();
                object Result = Command.ExecuteScalar();
                if (Result != null && int.TryParse(Result.ToString(), out int ID))
                {
                    InsertedID = ID;
                }
            }catch (Exception ex)
            {
                InsertedID=-1;
            }finally 
            {
                Connection.Close(); 
            }

            return InsertedID;

        }

        public static bool IsApplicationExists(int PersonID
            ,int LicenseTypeID,ref int ApplicationID)
        {

            ApplicationID = -1;
            string Querey = @"select Applications.ApplicationID 
                             from Applications inner join 
                            LocalDrivingLicenseApplications
                            on Applications.ApplicationID=
                            LocalDrivingLicenseApplications.ApplicationID
                            
                            where Applications.ApplicantPersonID=@PersonID and 
                            (Applications.ApplicationStatus=1 or 
                            Applications.ApplicationStatus=3) and 
                            (LocalDrivingLicenseApplications.LicenseClassID=
                             @LicenseTypeID);
                            ;";

            SqlConnection connection = new SqlConnection( Settings.ConnectionString );
            SqlCommand Command = new SqlCommand(Querey, connection);

            Command.Parameters.AddWithValue("@PersonID", PersonID);
            Command.Parameters.AddWithValue("@LicenseTypeID", LicenseTypeID);
            try
            {
                connection.Open();
                Object Result = Command.ExecuteScalar();

               if(Result!=null && int.TryParse(Result.ToString(),out int ID))
                {
                    ApplicationID=ID;
                }
            }catch(Exception ex)
            {
                ApplicationID=-1;
            }
            finally
            {
                connection.Close();
            }

            return ApplicationID != -1;


        }

        //public static int GetApplicationIDUsingPersonID(int PersonID)
        //{
        //    int ApplicationID = 0;
        //    string Querey = @"select Orders.OrderID from Orders 
        //                    where Orders.PersonID=@PersonID;
        //                    ";
        //    SqlConnection Connection=new SqlConnection( Settings.ConnectionString );
        //    SqlCommand Command=new SqlCommand(Querey, Connection);
        //    Command.Parameters.AddWithValue("@PersonID", PersonID);

        //    try
        //    {
        //        Connection.Open();
        //        Object Result = Command.ExecuteScalar();
        //        if(Result != null && int.TryParse(Result.ToString(),out int ID)) 
        //        {

        //            ApplicationID = ID;


        //        }
        //    }catch (Exception ex)
        //    {
        //        ApplicationID=0;
        //    }finally { Connection.Close(); }
        //    return ApplicationID;
        //}


        public static bool CancelOrder(int OrderID)
        {
            int NumberOfAffectedRows = 0;
            string Querey = @"update Applications 
                             set ApplicationStatus = 2 
                             where ApplicationID=@OrderID;";
            SqlConnection Connection = new SqlConnection(Settings.ConnectionString);
            SqlCommand Command = new SqlCommand( Querey, Connection);

            Command.Parameters.AddWithValue("@OrderID", OrderID);

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

        public static bool FindOrder(int OrderID,ref int PersonID,
            ref string AppStatus,ref DateTime AppDate,
            ref int OrderNameID)
        {
            bool IsFound = false;
            //string AppStatus = "";
            string Querey = @"select Applications.ApplicantPersonID,Status  
                              =   
                             case
                             when ApplicationStatus = 1 then 'New'
                             when ApplicationStatus = 2 then 'Cancelled'
                             when ApplicationStatus = 3 then  'Completed'
                             end,
                              Applications.ApplicationDate,
                              Applications.ApplicationTypeID
                              from Applications 
                              where Applications.ApplicationID=@OrderID;";
           
            SqlConnection connection = new SqlConnection(Settings.ConnectionString);
            SqlCommand Command = new SqlCommand(Querey, connection);
            Command.Parameters.AddWithValue("@OrderID", OrderID);

            try
            {
                connection.Open();
                SqlDataReader Reader = Command.ExecuteReader();
                if (Reader.Read())
                {
                    IsFound = true;
                    PersonID = (int)Reader["ApplicantPersonID"];
                    AppStatus = (string)Reader["Status"];
                   
                    AppDate = (DateTime)Reader["ApplicationDate"];
                    OrderNameID = (int)Reader["ApplicationTypeID"];

                }
                Reader.Close();

            }catch(Exception ex)
            {
                IsFound = false;
            }
            finally
            {
                connection.Close();
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

        public static bool GetApplicationFeesUsingName(string ServiceName , ref float Fees)
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
                    float.TryParse(Reader["ApplicationFees"].ToString(), out float fees);
                    Fees = fees;
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

        public static bool CompleteOrder(int OrderID)
        {
            int NumberOfAffectedRows = 0;
            string Querey = @"update Applications 
                             set ApplicationStatus = 3 
                             where ApplicationID=@OrderID;";
            SqlConnection Connection = new SqlConnection(Settings.ConnectionString);
            SqlCommand Command = new SqlCommand(Querey, Connection);

            Command.Parameters.AddWithValue("@OrderID", OrderID);

            try
            {
                Connection.Open();
                NumberOfAffectedRows = Command.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                NumberOfAffectedRows = 0;
            }
            finally
            {
                Connection.Close();
            }

            return NumberOfAffectedRows > 0;

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

        




    }
}
