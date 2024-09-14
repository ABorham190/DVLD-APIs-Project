using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Security.Cryptography;

namespace DVLDdataAccessLayer
{
    public class Settings
    {
        static public string SourceName = "MyDLApp Project 1";

        static public string ConnectionString = "Server=.;Database=DVLD;User Id=sa;" +
        "Password=111990;TrustServerCertificate=True;";
        //static public string ConnectionString = ConfigurationManager.AppSettings["ConnectionStringDefault"];
        static public void AddErrorToEventViewer(string Error,string exMessage)
        {
             string SourceName = "MyDLApp Project 1";
            string Message = Error + " , " + exMessage;
            if(!EventLog.SourceExists(SourceName))
            {
                EventLog.CreateEventSource(SourceName, "Application");
            }
            EventLog.WriteEntry(SourceName, Message, EventLogEntryType.Error);

        }
        static public void AddInfoToEventViewer(string Message)
        {
            string SourceName = "MyDLApp Project 1";
            
            if (!EventLog.SourceExists(SourceName))
            {
                EventLog.CreateEventSource(SourceName, "Application");
            }
            EventLog.WriteEntry(SourceName, Message, EventLogEntryType.Information);

        }

        static public int GetCountryID(string CountryName)
        {
            int CountryID = 0;
           string Querey=@" select CountryID from Countries where
                           CountryName = @CountryName;";
            SqlConnection Connection=new SqlConnection(ConnectionString);
            SqlCommand Command = new SqlCommand(Querey, Connection);
            Command.Parameters.AddWithValue("@CountryName", CountryName);

            try
            {
                Connection.Open();
                Object Result = Command.ExecuteScalar();
                if(Result!=null && int.TryParse(Result.ToString(),out int ID))
                {
                    CountryID = ID;
                }


            }catch (Exception ex)
            {
                CountryID=0;
            }
            finally
            {
                Connection.Close();
            }
            return CountryID;
        }

        public static string ComputeHash(string Input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(Input));

                // Convert the byte array to a lowercase hexadecimal string
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }


        static string Encrypt(string plainText, string key)
        {
            using (Aes aesAlg = Aes.Create())
            {
                // Set the key and IV for AES encryption
                aesAlg.Key = Encoding.UTF8.GetBytes(key);

                /*
                Here, you are setting the IV of the AES algorithm to a block of bytes 
                with a size equal to the block size of the algorithm divided by 8. 
                The block size of AES is typically 128 bits (16 bytes), 
                so the IV size is 128 bits / 8 = 16 bytes.
                 */
                aesAlg.IV = new byte[aesAlg.BlockSize / 8];


                // Create an encryptor
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);


                // Encrypt the data
                using (var msEncrypt = new System.IO.MemoryStream())
                {
                    using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (var swEncrypt = new System.IO.StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }

                    // Return the encrypted data as a Base64-encoded string
                    return Convert.ToBase64String(msEncrypt.ToArray());
                }
            }
        }

        static string Decrypt(string cipherText, string key)
        {
            using (Aes aesAlg = Aes.Create())
            {
                // Set the key and IV for AES decryption
                aesAlg.Key = Encoding.UTF8.GetBytes(key);
                aesAlg.IV = new byte[aesAlg.BlockSize / 8];


                // Create a decryptor
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);


                // Decrypt the data
                using (var msDecrypt = new System.IO.MemoryStream(Convert.FromBase64String(cipherText)))
                using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                using (var srDecrypt = new System.IO.StreamReader(csDecrypt))
                {
                    // Read the decrypted data from the StreamReader
                    return srDecrypt.ReadToEnd();
                }
            }
        }
    }
}
