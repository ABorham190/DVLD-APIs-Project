using AutoMapper;
using DVDLBussinessLayer;
using dvld_api.models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace dvld_api.Controllers
{
    [Route("api/Appointment")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly ILogger<AppointmentController> _logger;
        private readonly IMapper _mapper;

        public AppointmentController(ILogger<AppointmentController> logger,IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
        }

        [HttpGet("GetByID/{ID}",Name ="GetAppointmentByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public IActionResult GetByID(int ID)
        {
            _logger.LogInformation("Start GetByID method (AppointmentController)");
            if (ID < 1)
            {
                _logger.LogError($"Invalid User input {ID}");
                var response = new { message = $"Invalid User input {ID}" };
                return BadRequest(response);
            }

            try
            {
                clsAppointments Appointment = clsAppointments.Find(ID);

                if (Appointment == null)
                {
                    _logger.LogError($"Appointment with ID : {ID} is not found");
                    return NotFound($"There is no Appointment with ID : {ID}");
                }

                GetAppointmentDTO getAppointmentDTO = _mapper.Map<GetAppointmentDTO>(Appointment)!;
                if (getAppointmentDTO == null)
                {
                    _logger.LogError("Mapping failed for Appointment ID: {ID}", ID);
                    return StatusCode(500, new { error = "Mapping error" });
                }

                _logger.LogInformation("Appointment founded and mapped successfully {@getAppointmentDTO}", getAppointmentDTO);

                return Ok(getAppointmentDTO);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexepected Error occured "+ ex.Message);
                return StatusCode(500, new { error = "internal server Error" });
            }


        }



        [HttpPost("AddAnApointment")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> AddAnAppointment([FromBody] AddAppointmentDTO appointmentDTO)
        {
            _logger.LogInformation("Start AddAnAppointment method in AppointmentController ");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                if (!await LDLApp.IsThisLDLAppIDAllowedToBookTest(appointmentDTO.LDLAppID, appointmentDTO.TestTypeID))
                {
                    _logger.LogError("Not Allowed To Book This Test");
                    return BadRequest(new { error = "This LDLAppID is not allowed to Book This test" });
                }

                clsAppointments Appointment = _mapper.Map<clsAppointments>(appointmentDTO);
                if (Appointment == null)
                {
                    _logger.LogError("Mapping Error");

                    return StatusCode(500, new { error = "Mapping Error" });
                }

                if (!Appointment!.Save())
                {
                    _logger.LogError("Error in Saving Appointment");
                    return StatusCode(500, new { error = "Error in saving Appointment" });
                }

                _logger.LogInformation($"Appointment added successfully with ID : {Appointment.AppointmentID}");

                return CreatedAtRoute("GetAppointmentByID", new {ID=Appointment.AppointmentID},Appointment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in AddAnAppointment method in AppointmentController");
                return StatusCode(500, new { Message = "Internal server Error", Error = ex.Message });
            }

        }
    }
}
