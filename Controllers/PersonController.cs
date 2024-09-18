using AutoMapper;
using DVDLBussinessLayer;
using DVLDdataAccessLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Microsoft.VisualBasic.FileIO;


namespace dvld_api.Controllers
{
    [Route("api/Person")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly IMapper _mapper;

        public PersonController(IMapper mapper)
        {
            _mapper = mapper;
        }

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

        [HttpGet("GetByID/{PersonID}",Name ="GetByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public IActionResult GetByID(int PersonID)
        {
            if (PersonID < 1)
            {
                return BadRequest("Invalid input parameters");
            }

            clsPerson person = clsPerson.FindPerson(PersonID);

            if (person == null)
            {
                return BadRequest($"There is no person with PersonID : {PersonID}");
            }
            PersonDTO persondto=_mapper.Map<PersonDTO>(person);

            return Ok(persondto);

        }

        [HttpGet("GetByNationalNumber/{NationalNo}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public IActionResult GetByNationalNumber(string NationalNo)
        {
            if (string.IsNullOrEmpty(NationalNo))
            {
                return BadRequest("Invalid input parameters");
            }

            clsPerson person = clsPerson.FindPerson(NationalNo);

            if (person == null)
            {
                return BadRequest($"There is no person with PersonID : {NationalNo}");
            }
            PersonDTO persondto = _mapper.Map<PersonDTO>(person);

            return Ok(persondto);

        }

        [HttpPost("AddNew")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public IActionResult AddNew(AddNewPersonDTO newPerson)
        {
            if (newPerson==null)
            {
                return BadRequest("Invalid input parameters");
            }

            if (string.IsNullOrEmpty(newPerson.FirstName) || string.IsNullOrEmpty(newPerson.SecondName) ||
                string.IsNullOrEmpty(newPerson.LastName) || string.IsNullOrEmpty(newPerson.Address) ||
                 string.IsNullOrEmpty(newPerson.Phone) || string.IsNullOrEmpty(newPerson.NationalNumber)
                 || string.IsNullOrEmpty(newPerson.GenderType) || 
                 newPerson.NationalityCountryID < 1 || newPerson.NationalityCountryID > 193)
            {
                return BadRequest("Invalid User Input");
            }
           
            clsPerson person = _mapper.Map<clsPerson>(newPerson);



            if (person == null)
            {
                return StatusCode(500, new { error = "Internal server Error" });

            }

            person.Gender = newPerson.GenderType == "Male" ? 1 : 0;

            if (!person.Save())
            {
                return StatusCode(500, new { error = "Internal server error" });
            }

            return CreatedAtAction(nameof(GetByID), new {PersonID=person.ID},newPerson);
        }

        [HttpPut("UpdateByID/{PersonID}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public IActionResult UpdateByID(int PersonID,UpdatePersonDTO UpdatedPerson)
        {
            if (PersonID < 1)
            {
                return BadRequest("Invalid User input");
            }

            clsPerson Person=clsPerson.FindPerson(PersonID);

            if (Person == null)
            {
                return NotFound($"There is no Person with PersonID : {PersonID}");
            }

            if (UpdatedPerson == null)
            {
                return BadRequest("Invalid input parameters");
            }

            if ( string.IsNullOrEmpty(UpdatedPerson.Address) ||string.IsNullOrEmpty(UpdatedPerson.Phone))
            {
                return BadRequest("Invalid User Input");
            }

            Person.Address = UpdatedPerson.Address;
            Person.Phone = UpdatedPerson.Phone;
            Person.Email = UpdatedPerson.Email;
            Person.ImagePath = UpdatedPerson.ImagePath;

            if (!Person.Save())
            {
                return StatusCode(500, new { error = "Internal server Error" });
            }

            PersonDTO personDTO = _mapper.Map<PersonDTO>(Person);

            return Ok(personDTO);

        }
    }
}
