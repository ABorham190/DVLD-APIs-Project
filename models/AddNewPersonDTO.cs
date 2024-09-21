namespace dvld_api.models
{
    public class AddNewPersonDTO
    {
        public string NationalNumber { set; get; }
        public string FirstName { set; get; }
        public string SecondName { set; get; }
        public string ThirdName { set; get; }
        public string LastName { set; get; }

        public string GenderType { set; get; }

        public DateTime DateOfBirth { set; get; }
        public string Address { set; get; }
        public string Phone { set; get; }
        public string Email { set; get; }
        public int NationalityCountryID { set; get; }
        public required IFormFile PersonPhoto { set; get; }
        

    }
}
