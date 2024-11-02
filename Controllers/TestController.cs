using DVDLBussinessLayer;
using DVLDdataAccessLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace dvld_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class TestController : ControllerBase
    {
        private readonly ILogger _logger;

        public TestController(ILogger logger)
        {
            _logger = logger;
        }

        [HttpPost("TakeTest")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> TakeTest([FromBody]TakeTestDto takenTest)
        {
            _logger.LogInformation("Start executing TakeTest Method in TestController");

            if (takenTest == null)
            {
                _logger.LogError("taken test equals null");
                return BadRequest("Invalid user input");
            }

            try
            {
                if (await clsTests.TakeTest(takenTest))
                {
                    _logger.LogInformation("Test taken successfully");
                    return Ok("Test Taken Successfully");
                }
                var response = new
                {
                    code = 500,
                    message = "Internal server error"
                };
                _logger.LogError("test not taken successfully");
                return StatusCode(500, response);
            }
            catch (Exception ex)
            {
                _logger.LogError("Unexcepected error occured " + ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }
    }
}
