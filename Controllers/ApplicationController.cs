using AutoMapper;
using DVDLBussinessLayer;
using dvld_api.models;
using DVLDdataAccessLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace dvld_api.Controllers
{
    [Route("api/Application")]
    [ApiController]
    public class ApplicationController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<ApplicationController> _logger;
        public ApplicationController(IMapper mapper, ILogger<ApplicationController> logger)
        {
            _mapper = mapper;
            _logger = logger;
        }
        enum AppStatus { New=1,Cancelled=2,Completed=3}

        [HttpPatch("Cancel/{AppID}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<IActionResult> Cancel(int AppID)
        {
            
            _logger.LogInformation("Starting Cancel Method in ApplicationController");

            try
            {
                if (AppID < 1)
                {
                    _logger.LogError($"INVALID user input {AppID}");
                    return BadRequest("Invalid User input");
                }

                clsOrders order = clsOrders.FindApplicationByID(AppID);

                if (order == null)
                {
                    _logger.LogError($"There is no application with ID : {AppID}");
                    return NotFound($"There is no application with ID : {AppID}");
                }

                switch ((AppStatus)order.ApplicationStatus)
                {
                    case AppStatus.Cancelled:
                        _logger.LogError($"This Application Already cancelled with ApplicantID : {order.ApplicantID}");
                        return BadRequest("This Application Already Cancelled");

                    case AppStatus.Completed:
                        _logger.LogError($"You Cant Cancel This Application Because It Is Completed with ApplicantID : {order.ApplicantID}");
                        return BadRequest("You Cant Cancel This Application Because It Is Completed");
                }


                if (!await clsOrders.UpdateApplicationStatus(AppID,clsOrdersDataLayer.enWhatToDo.Cancel))
                {
                    _logger.LogError("Error In CancelApplication method ");
                    return StatusCode(500, new { error = "Internal server Error" });
                }


                order = clsOrders.FindApplicationByID(AppID);

                
                clsApplicationDTO appdto = _mapper.Map<clsApplicationDTO>(order);

                if(appdto != null)
                _logger.LogInformation("Application Canceled successfully : {@appdto}",appdto);


                return Ok(appdto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Execution of Cancel method in Application Controller");
                return StatusCode(500, new { error = "Internal server error" });
            }

        }

        [HttpPatch("Complete/{AppID}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<IActionResult> Complete(int AppID)
        {

            _logger.LogInformation("Starting Complete Method in ApplicationController");

            try
            {
                if (AppID < 1)
                {
                    _logger.LogError($"INVALID user input {AppID}");
                    return BadRequest("Invalid User input");
                }

                clsOrders order = clsOrders.FindApplicationByID(AppID);

                if (order == null)
                {
                    _logger.LogError($"There is no application with ID : {AppID}");
                    return NotFound($"There is no application with ID : {AppID}");
                }

                switch ((AppStatus)order.ApplicationStatus)
                {
                    case AppStatus.Cancelled:
                        _logger.LogError("This Application cant completed , its cancelled  : {@order}",order);
                        return BadRequest("Cant Complete Canceled Application");

                    case AppStatus.Completed:
                        _logger.LogError("This Application Already Completed : {@order}",order);
                        return BadRequest("This Application Already Completed");
                }


                if (!await clsOrders.UpdateApplicationStatus(AppID, clsOrdersDataLayer.enWhatToDo.Complete))
                {
                    _logger.LogError("Error In UpdateApplication method ");
                    return StatusCode(500, new { error = "Internal server Error" });
                }


                order = clsOrders.FindApplicationByID(AppID);


                clsApplicationDTO appdto = _mapper.Map<clsApplicationDTO>(order);

                if (appdto != null)
                    _logger.LogInformation("Application Completed successfully : {@appdto}", appdto);


                return Ok(appdto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Execution of Complete method in Application Controller");
                return StatusCode(500, new { error = "Internal server error" });
            }

        }


    }
}
