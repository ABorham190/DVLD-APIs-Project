using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using DVLDdataAccessLayer;


namespace DVDLBussinessLayer
{
    public class clsPerson
    {
        public string NationalNumber {  get; set; }
        public string FirstName {  get; set; }
        public string SecondName {  get; set; }
        public string ThirdName {  get; set; }
        public string LastName {  get; set; }
        public string Phone {  get; set; }
        public string Email { get; set; }
        public string Nationality {  get; set; }
        public int Gender {  get; set; }
        public string Address {  get; set; }
        public string ImagePath {  get; set; }
        public int ID {  get; set; }
        public DateTime DateOfBirth { get; set; }
        public string Country {  get; set; }
        public string FullName {  get; set; }
        public int NationalityCountryID { get; set; }

       public enum enMode { enAddNewMode=0,enUpdateMode=1}
        public enMode _Mode;

       public  clsPerson()
        {
            NationalNumber = "";
            FirstName = "";
            SecondName = "";
            ThirdName = "";
            LastName = "";
            Phone = "";
            Email = "";
            Nationality = "";
            Gender =-1;
            Address = "";
            ImagePath = "";
            ID = -1;
            DateOfBirth = DateTime.Now;    

            _Mode=enMode.enAddNewMode;

        }
        private clsPerson(int id,string nationalNumber,string firstName,string secondName,
            string thirdName,string lastName,string email,string address,
            string phone,int gender,int natioCountryID,DateTime dateofbirth,
            string imagepath,string country)
        {
            this.ID = id;
            this.NationalNumber = nationalNumber;
            this.FirstName = firstName;
            this.LastName = lastName;
            this.ThirdName = thirdName;
            this.SecondName = secondName;
            this.Email = email;
            this.Address = address;
            this.Phone = phone;
            this.Gender = gender;
            this.Country = country;
            this.NationalityCountryID = natioCountryID;
            this.DateOfBirth = dateofbirth;
            this.ImagePath = imagepath;


            this.FullName=FirstName+" "+SecondName+" "+ThirdName+" "+LastName;
            _Mode = enMode.enUpdateMode;

        }

        private async Task<bool> _AddNewPerson()
        {

            this.ID=await clsPeopleDataLayer.AddNewPerson(this.NationalNumber,
                this.FirstName, this.SecondName, this.ThirdName,
                this.LastName, this.DateOfBirth, this.Address,
                this.Phone, this.Email, this.NationalityCountryID ,
                this.Gender,
                this.ImagePath);
            
           return this.ID !=-1;
            
        }
        private bool _UpdatePersonInfo()
        {

            return clsPeopleDataLayer.UpdatePersonInfo(this.ID,
                this.NationalNumber,
                this.FirstName, this.SecondName, this.ThirdName,
                this.LastName,
                this.Phone, this.Email, this.Address, this.NationalityCountryID,
                this.Gender, this.ImagePath,this.DateOfBirth);

        }

        public static List<PersonDTO> GetAllPersonsData()
        {
            return clsPeopleDataLayer.GetPersonsList();
        }
        public static async Task <bool> IsPersonExistsInSystemAsync(string NationalNumber)
        {
            return await clsPeopleDataLayer.IsPersonExistInDatabaseAsync(NationalNumber);
        }

        public async Task<bool> Save()
        {

            switch (_Mode)
            {

                case enMode.enAddNewMode:
                    if (await _AddNewPerson())
                    {
                        
                        return true;

                    }
                    else
                    {
                        return false;
                    }
                case enMode.enUpdateMode:

                    if (_UpdatePersonInfo())
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                    
            }
            return false;
        }
        
        public static clsPerson FindPerson(int PersonID)
        {
            string NationalNumber = "", FirstName = "", SecondName = "",
                ThirdName = "", LastName = "", Email = "", Phone = "",
                Address = "",  ImagePath = "";
            int Gender = -1;
            string Country= "";
            int NatiocountID = -1;
            DateTime DateOfBirth = DateTime.Now;
            if(clsPeopleDataLayer.FindPerson(PersonID,ref NationalNumber,
                ref FirstName,ref SecondName,ref ThirdName,ref LastName,
                ref Gender,ref Email,ref Phone,ref Address,ref ImagePath,
                ref NatiocountID, ref DateOfBirth,ref Country))
            {
                return new clsPerson(PersonID,NationalNumber,FirstName,
                    SecondName,ThirdName,LastName,Email,Address,Phone,Gender,
                    NatiocountID, DateOfBirth,ImagePath,Country);

            }
            else
            {
                return null;
            }
        }
        public static clsPerson FindPerson(string NationalNumber)
        {
            string FirstName = "", SecondName = "",
                ThirdName = "", LastName = "", Email = "", Phone = "",
                Address = "", Country = "", ImagePath = "";
            int Gender = -1;
            int PersonID = -1;
            int CountryID = -1;
            DateTime DateOfBirth = DateTime.Now;
            if (clsPeopleDataLayer.FindPersonByNationalNumber(ref PersonID,
                NationalNumber,
                ref FirstName, ref SecondName, ref ThirdName, ref LastName,
                ref Gender, ref Email, ref Phone, ref Address, ref ImagePath,
                ref CountryID, ref DateOfBirth,ref Country))
            {
                return new clsPerson(PersonID, NationalNumber, FirstName,
                    SecondName, ThirdName, LastName, Email, Address, Phone, Gender,
                    CountryID, DateOfBirth, ImagePath,Country);

            }
            else
            {
                return null;
            }
        }
        public static bool IsPersonAUser(int PersonID)
        {
            return clsPeopleDataLayer.IsPersonAuser(PersonID);
        }


        public static bool DeletePerson(int PersonID)
        {
            return clsPeopleDataLayer.DeletePerson(PersonID);
        }

        public static bool GetFullName(int PersonID,ref string FullName)
        {
            return clsPeopleDataLayer.GetPersonFullName(PersonID, ref FullName);
        }
        public static bool GetPersonIDUsingLDLAppID(int LDLAppID,ref int PersonID)
        {
            return clsPeopleDataLayer.GetPersonIDUsingLDLAppID(LDLAppID, ref PersonID);
            
        }

        public static bool IsThisPersonADriver(int PersonID,ref int DriverID)
        {
            return clsDriversDataLayer.IsThisPersonADriver(PersonID,ref DriverID);
        }
        public static bool GetPersonIDUsingNationalNumber(string NationalNo,ref int PersonID)
        {
            return clsPeopleDataLayer.GetPersonIDUsingNationalNo(NationalNo, ref PersonID);
        }
        public static int GetPersonIDByLicenseID(int LicenseID)
        {
            return clsPeopleDataLayer.GetPersonIDUsingLicenseID(LicenseID);
        }

        public static bool AddPreviousPersonImageToDataBase(int PersonID,string ImagePath,DateTime DateOfChange)
        {
            return clsPersonImagesHandling.AddPerviousImageToPreviousImageTable(PersonID, ImagePath, DateOfChange);
        }
    }
}
