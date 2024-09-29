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



        [HttpPost("AddAnApointment")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<IActionResult> AddAnAppointment([FromBody] AppointmentDTO appointmentDTO)
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

                if (!Appointment.Save())
                {
                    _logger.LogError("Error in Saving Appointment");
                    return StatusCode(500, new { error = "Error in saving Appointment" });
                }

                _logger.LogInformation($"Appointment added successfully with ID : {Appointment.AppointmentID}");

                return Created();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in AddAnAppointment method in AppointmentController");
                return StatusCode(500, new { Message = "Internal server Error", Error = ex.Message });
            }

        }
    }
}
