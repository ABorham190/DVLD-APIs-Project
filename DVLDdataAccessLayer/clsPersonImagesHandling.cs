using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;

namespace DVLDdataAccessLayer
{
    public static class clsPersonImagesHandling
    {
        public static bool AddPerviousImageToPreviousImageTable(int PersonID, string ImagePath, DateTime DateOfChange)
        {
            int NumberOfAffectedRows = 0;
            try
            {
                using (SqlConnection Connection = new SqlConnection(Settings.ConnectionString))
                {
                    Connection.Open();
                    using (SqlCommand Command = new SqlCommand("SP_AddImageToPreviousImages", Connection))
                    {
                        Command.CommandType = CommandType.StoredProcedure;

                        Command.Parameters.AddWithValue("@PersonID", PersonID);
                        Command.Parameters.AddWithValue("ImagePath", ImagePath);
                        Command.Parameters.AddWithValue("DateOfChange", DateOfChange);

                        NumberOfAffectedRows = Command.ExecuteNonQuery();

                    }
                }
            }
            catch (Exception ex)
            {
                NumberOfAffectedRows = 0;
                Settings.AddErrorToEventViewer("Error : ", ex.Message);
            }

            return NumberOfAffectedRows > 0;
        }
    }
}
