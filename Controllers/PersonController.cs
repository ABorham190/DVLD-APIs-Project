using AutoMapper;
using DVDLBussinessLayer;
using DVLDdataAccessLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;


namespace dvld_api.Controllers
{
    [Route("api/Person")]
    [ApiController]
    public class PersonController : ControllerBase
    {
       

        [HttpGet("GetAll")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]

        public ActionResult<List<PersonDTO>> GetAll()
        {
            List<PersonDTO>personsList = new List<PersonDTO>();
            personsList=clsPerson.GetAllPersonsData();

            if (personsList.Count > 0)
            {
                return Ok(personsList);
            }

            return NoContent();
            
        }

        [HttpGet("GetByID/{PersonID}")]

        public IActionResult GetByID(int PersonID)
        {
            if (PersonID < 1)
            {
                return BadRequest("Invalic input parameters");
            }

            clsPerson person = clsPerson.FindPerson(PersonID);

            if (person == null)
            {
                return BadRequest($"There is no person with PersonID : {PersonID}");
            }

            PersonDTO persondto = new PersonDTO
            {
                ID = person.ID,
                NationalNumber = person.NationalNumber,
                FullName = person.FullName,
                DateOfBirth = person.DateOfBirth,
                Gender = person.Gender == 1 ? "Male" : "Female",
                Phone = person.Phone,
                Email = person.Email,
                Country = person.Country,

            };

            return Ok(persondto);

        }
    }
}
