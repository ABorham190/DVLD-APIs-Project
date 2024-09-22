using AutoMapper;
using DVDLBussinessLayer;
using dvld_api.models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace dvld_api.Controllers
{
    [Route("api/Application")]
    [ApiController]
    public class ApplicationController : ControllerBase
    {
        private readonly IMapper _mapper;
        public ApplicationController(IMapper mapper) 
        {
            _mapper = mapper;
        }
        enum AppStatus { New=1,Cancelled=2,Completed=3}

        [HttpPut("Cancel/{AppID}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public IActionResult Cancel(int AppID)
        {
            clsOrders order=clsOrders.FindOrder(AppID);

            if (order == null)
            {
                return BadRequest($"There is no application with ID : {AppID}");
            }

            switch ((AppStatus)order.ApplicationStatus)
            {
                case AppStatus.Cancelled:
                    return BadRequest("This Application Already Cancelled");

                case AppStatus.Completed:
                    return BadRequest("You Cant Cancel This Application Because It Is Completed");
            }
            

            if (!clsOrders.CancelApplication(AppID))
            {
                return StatusCode(500, new { error = "Internal server Error" });
            }

            order = clsOrders.FindOrder(AppID);

            clsApplicationDTO appdto = _mapper.Map<clsApplicationDTO>(order);



            return Ok(appdto);

        }
    }
}
