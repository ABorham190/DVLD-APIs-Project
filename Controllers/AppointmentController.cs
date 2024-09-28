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
        public async Task<IActionResult> AddAnAppointment([FromBody]AppointmentDTO appointmentDTO)
        {
            _logger.LogInformation("Start AddAnAppointment method in AppointmentController ");

            if (!ModelState.IsValid)
            {
                _logger.LogError("Invalid Parameters");
                return BadRequest("Invalid Parameters");
            }

            if (appointmentDTO.LDLAppID < 1)
            {
                _logger.LogError("Invalid User Input {@appointmentDTo}", appointmentDTO);
                return BadRequest("Invalid User input");
            }

            if (!await LDLApp.IsThisLDLAppIDAllowedToBookTest(appointmentDTO.LDLAppID, appointmentDTO.TestTypeID))
            {
                _logger.LogError("Not Allowed To Book This Test");
                return BadRequest(new { error = "This LDLAppID is not allowed to Book This test" });
            }

            clsAppointments Appointment=_mapper.Map<clsAppointments>(appointmentDTO);

            if (!Appointment.Save())
            {
                _logger.LogError("Error in Saving Appointment");
                return StatusCode(500, new { error = "Error in saving Appointment" });
            }

            _logger.LogInformation($"Appointment added successfully with ID : {Appointment.AppointmentID}");

            return Created();

        }
    }
}
