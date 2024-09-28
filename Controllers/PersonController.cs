using AutoMapper;
using DVDLBussinessLayer;
using DVLDdataAccessLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using Microsoft.VisualBasic.FileIO;
using dvld_api.models;


namespace dvld_api.Controllers
{
    [Route("api/Person")]
    [ApiController]
    public class PersonController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<PersonController> _logger;

        public PersonController(IMapper mapper, ILogger<PersonController> logger)
        {
            _mapper = mapper;
            _logger = logger;
        }

        [HttpGet("GetAll")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]

        public ActionResult<List<PersonDTO>> GetAll()
        {
            try
            {
                List<PersonDTO> personsList = new List<PersonDTO>();
                personsList = clsPerson.GetAllPersonsData();

                if (personsList.Count > 0)
                {
                    return Ok(personsList);
                }

                return NoContent();
            }
            catch (Exception ex) 
            {
                Settings.AddErrorToEventViewer("Error : ", ex.Message);
                return StatusCode(500, new { error = "Internal server error" });
            }
            
        }

        [HttpGet("GetByID/{PersonID}",Name ="GetByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public IActionResult GetByID(int PersonID)
        {
            try
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
                
                PersonDTO persondto = _mapper.Map<PersonDTO>(person);

                

                return Ok(persondto);
            }
            catch (Exception ex) 
            {

                Settings.AddErrorToEventViewer("Error is : ",ex.Message);
                return StatusCode(500, new { error = "Internal server error" });
            
            }

        }

        [HttpGet("GetByNationalNumber/{NationalNo}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public IActionResult GetByNationalNumber(string NationalNo)
        {
            try
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
            }catch(Exception ex)
            {
                Settings.AddErrorToEventViewer("Error : ", ex.Message);
                return StatusCode(500, new { error = "Internal server error" });
            }

        }

        [HttpPost("AddNew")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> AddNew(AddNewPersonDTO newPerson)
        {
            _logger.LogInformation("Starting AddNew method inside PersonConroller");

            if (!ModelState.IsValid)
            {
                _logger.LogError("One of enterd parameters is empty or equal null : {@newPerson}", newPerson);
                return BadRequest(ModelState);
            }

            try
            {

                if (await clsPerson.IsPersonExistsInSystemAsync(newPerson.NationalNumber))
                {
                    _logger.LogError("There is person with the same NationalNum in the system");
                    return BadRequest("There is person with this National Number in the system!!");
                }

                string ImagePath = string.Empty;
                if (newPerson.PersonPhoto != null)
                {
                    ImagePath = await clsUploadPersonPhoto.Upload(newPerson.PersonPhoto);
                }

                _logger.LogInformation("All entered parameters are valid {@newPerson}", newPerson);

                clsPerson person = _mapper.Map<clsPerson>(newPerson);

                _logger.LogInformation("Person and newPerson Mapped successfully");

                if (person == null)
                {
                    _logger.LogError("person equal null");
                    return StatusCode(500, new { error = "Internal server Error" });

                }

                person.Gender = newPerson.GenderType == "Male" ? 1 : 0;
                person.ImagePath = ImagePath;

                if (!await person.Save())
                {
                    _logger.LogInformation("Error in saving Person");
                    return StatusCode(500, new { error = "Internal server error" });
                }

                _logger.LogInformation($"Person Saved successfully With ID : {person.ID}");

                return CreatedAtAction(nameof(GetByID), new { PersonID = person.ID }, newPerson);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Unexpected error in AddNew method in PersonController");
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        [HttpPut("UpdateByID/{PersonID}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task< IActionResult> UpdateByID(int PersonID,UpdatePersonDTO UpdatedPerson)
        {
            try
            {
                if (PersonID < 1)
                {
                    return BadRequest("Invalid User input");
                }

                clsPerson Person = clsPerson.FindPerson(PersonID);

                if (Person == null)
                {
                    return NotFound($"There is no Person with PersonID : {PersonID}");
                }

                string OldImagePath = string.IsNullOrEmpty(Person.ImagePath)?"":Person.ImagePath;

                if (UpdatedPerson == null)
                {
                    return BadRequest("Invalid input parameters");
                }

                if (string.IsNullOrEmpty(UpdatedPerson.Address) || string.IsNullOrEmpty(UpdatedPerson.Phone)
                    ||UpdatedPerson.PersonImage==null)
                {
                    return BadRequest("Invalid User Input");
                }
                

                Person.Address = UpdatedPerson.Address;
                Person.Phone = UpdatedPerson.Phone;
                Person.Email = UpdatedPerson.Email;
                Person.ImagePath = await clsUploadPersonPhoto.Upload(UpdatedPerson.PersonImage);

                if (!await Person.Save())
                {
                    return StatusCode(500, new { error = "Internal server Error" });
                }

                PersonDTO personDTO = _mapper.Map<PersonDTO>(Person);

                if (!clsPersonImagesHandling.AddPerviousImageToPreviousImageTable
                    (Person.ID, OldImagePath, DateTime.Now))
                {

                    var response = new
                    {
                        message = "Old Image Not saved successfully",
                    };

                    return StatusCode(500,response);
                }

                return Ok(personDTO);
            }
            catch (Exception ex)
            {
                Settings.AddErrorToEventViewer("Error : ", ex.Message);
                return StatusCode(500, new { error = "Internal server error" });
            }

        }

        [HttpDelete("Delete/{PersonID}")]

        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]

        public IActionResult Delete(int PersonID)
        {
            try
            {
                if (PersonID < 1)
                {
                    return BadRequest("Invalid User Input");
                }

                clsPerson Person = clsPerson.FindPerson(PersonID);

                if (Person == null)
                {
                    return NotFound($"There is no Person Wiht PersonID : {PersonID}");
                }

                if (!clsPerson.DeletePerson(PersonID))
                {
                    return StatusCode(500, new { error = "Internal Server Error" });
                }

                return NoContent();
            }catch(Exception ex)
            {
                Settings.AddErrorToEventViewer("Error : ", ex.Message);
                return StatusCode(500, new { error = "Internal server error" });
            }
        }

        [HttpGet("GetPersonIDByNationalNumber/{NationalNo}")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]

        public IActionResult GetPersonIDByNationalNumber(string NationalNo)
        {
            try
            {
                if (string.IsNullOrEmpty(NationalNo))
                {
                    return BadRequest("Invalid User Input");
                }
                clsPerson Person = clsPerson.FindPerson(NationalNo);

                if (Person == null)
                {
                    return NotFound($"There is no Person Has National Number : {NationalNo}");
                }

                return Ok(Person.ID);
            }catch(Exception ex)
            {
                Settings.AddErrorToEventViewer("Error : ", ex.Message);
                return StatusCode(500, new { error = "Internal server error" });
            }
        }
    }
}
