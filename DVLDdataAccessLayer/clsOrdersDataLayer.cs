using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Windows.Forms;
using Microsoft.Data.SqlClient;

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

        public static int AddNewApplication(int ApplicantPersonID,
            DateTime ApplicationDate,
            int ApplicationTypeID,byte ApplicationStatus,
            DateTime LastStatusDate , Decimal PaidFees,int CreatedByUserID)
        {
            int InsertedID = -1;
            try {
                using (SqlConnection Connection = new SqlConnection(Settings.ConnectionString))
                {
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



                        Connection.Open();
                        int NumberOfAffectedRows = 0;
                        if ((NumberOfAffectedRows = Command.ExecuteNonQuery()) > 0)
                        {
                            InsertedID = (int)outputparam.Value;
                        }
                    }
                }

            }catch (Exception ex)
            {
                InsertedID=-1;
            }

            return InsertedID;

        }

        public static bool IsThisPersonIDHasAnActiveApplicationForThisLicenseTypeID(int PersonID
            ,int LicenseTypeID,ref int ApplicationID)
        {

            ApplicationID = -1;
            try {
                using (SqlConnection connection = new SqlConnection(Settings.ConnectionString))
                {
                    using (SqlCommand Command = new SqlCommand("SP_GetApplicationIDByPersonIDAndLicenseTypeID", connection))
                    {

                        Command.CommandType = CommandType.StoredProcedure;

                        Command.Parameters.AddWithValue("@PersonID", PersonID);
                        Command.Parameters.AddWithValue("@LicenseTypeID", LicenseTypeID);


                        connection.Open();
                        using (SqlDataReader Reader = Command.ExecuteReader())
                        {

                            if (Reader.Read())
                            {

                                ApplicationID = (int)Reader["ApplicationID"];
                            }
                        }
                    }
                }
            }catch(Exception ex)
            {
                ApplicationID=-1;
            }
           

            return ApplicationID != -1;


        }
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
            ref DateTime AppDate,
            ref int OrderNameID,ref byte ApplicationStatusID)
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
                              Applications.ApplicationTypeID,
                              ApplicationStatus
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
                    
                   
                    AppDate = (DateTime)Reader["ApplicationDate"];
                    OrderNameID = (int)Reader["ApplicationTypeID"];
                    ApplicationStatusID = (byte)Reader["ApplicationStatus"];

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
