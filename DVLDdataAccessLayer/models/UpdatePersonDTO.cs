namespace dvld_api.models
{
    public class UpdatePersonDTO
    {

        public string Address { set; get; }
        public string Phone { set; get; }
        public string Email { set; get; }

        public IFormFile PersonImage { set; get; }




    }
}
