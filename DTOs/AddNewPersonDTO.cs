using System.ComponentModel.DataAnnotations;

namespace dvld_api.models
{
    public class AddNewPersonDTO
    {
        [Required(ErrorMessage ="National Number is required")]
        [RegularExpression(@"^[a-zA-Z0-9]{5,14}$", ErrorMessage = "National Number must be between 5 and 14 alphanumeric characters.")]

        public string NationalNumber { set; get; }

        [Required(ErrorMessage = "FirstName is required")]
        [RegularExpression(@"^[a-zA-Z]{2,50}", ErrorMessage = "FirstName must contain only letters , and between 2 to 50 characters")]
        public string FirstName { set; get; }

        [Required(ErrorMessage ="Second Name is required")]
        [RegularExpression(@"^[a-zA-Z]{2,50}",ErrorMessage ="Second Name must contain only letters and between 2 , 50 characters")]
        public string SecondName { set; get; }
        public string ThirdName { set; get; }

        [Required(ErrorMessage ="Last Name is required")]
        [RegularExpression(@"^[a-zA-Z]{2,50}",ErrorMessage ="Last Name must contain only letters and between 2,50 characters ")]
        public string LastName { set; get; }

        [Required(ErrorMessage ="GetderType is required")]
        [RegularExpression("^(Male|Female)$",ErrorMessage ="Gender must be 'Male' or 'Female'")]
        public string GenderType { set; get; }

        [MinimumAge(18)]
        public DateTime DateOfBirth { set; get; }

        [Required(ErrorMessage ="Address is required")]
        public string Address { set; get; }


        [Required(ErrorMessage ="Phone is required")]
        [Phone(ErrorMessage ="Invalid Phone Number")]
        public string Phone { set; get; }

        public string Email { set; get; }

        [Required(ErrorMessage ="Nationality Country ID required")]
        [Range(1,193,ErrorMessage ="Nationality Country ID must be beween 1,193")]
        public int NationalityCountryID { set; get; }
        public required IFormFile PersonPhoto { set; get; }
        

    }
}
