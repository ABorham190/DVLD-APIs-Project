using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using dvld_api.models;
using AutoMapper;
using DVDLBussinessLayer;

namespace dvld_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetainLicenseController : ControllerBase
    {
        private readonly ILogger<DetainLicenseController> _logger;
        private readonly IMapper _mapper;

        public DetainLicenseController(ILogger<DetainLicenseController> logger,IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
        }

        [HttpPost("AddNew")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddNew(DetainLicenseDTO detainLicenseDTO)
        {
            _logger.LogInformation("Start executing AddNew DetainLicenseController");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var NewDetainID = HandleAddNewDetain.AddNewDetain(detainLicenseDTO);

                if (await NewDetainID!=-1)
                {
                    _logger.LogInformation($"Detain added successfully with ID : {NewDetainID}");
                    return Ok(NewDetainID);
                }
                else
                {
                    _logger.LogError($"Error in saving new detain");
                    return StatusCode(500, "Internal server error");
                }

            } catch (Exception ex) 
            {
                _logger.LogError(ex,"UnExcepected Error "+ex.Message);
                var response = new
                {
                    error = "Internal server Error",
                    code = 500
                };
                return StatusCode(500, response);
            }
        }
    }
}
