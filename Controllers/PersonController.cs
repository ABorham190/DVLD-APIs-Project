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

        public PersonController(IMapper mapper)
        {
            _mapper = mapper;
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
            try
            {
                if (newPerson == null)
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

                string ImagePath = "";
                if (newPerson.PersonPhoto == null)
                {
                    ImagePath = "";
                }
                else
                {
                    ImagePath = await clsUploadPersonPhoto.Upload(newPerson.PersonPhoto);

                }

                clsPerson person = _mapper.Map<clsPerson>(newPerson);



                if (person == null)
                {
                    return StatusCode(500, new { error = "Internal server Error" });

                }

                person.Gender = newPerson.GenderType == "Male" ? 1 : 0;
                person.ImagePath = ImagePath;

                if (!person.Save())
                {
                    return StatusCode(500, new { error = "Internal server error" });
                }

                return CreatedAtAction(nameof(GetByID), new { PersonID = person.ID }, newPerson);
            }
            catch (Exception ex)
            {
                Settings.AddErrorToEventViewer("Error : ", ex.Message);
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

                if (!Person.Save())
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
