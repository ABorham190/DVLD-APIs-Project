using DVDLBussinessLayer;
using dvld_api.models;
using DVLDdataAccessLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace dvld_api.Controllers
{
    [Route("api/LDLApp")]
    [ApiController]
    public class LDLAppController : ControllerBase
    {
        private readonly ILogger<LDLAppController> _logger;
        public LDLAppController(ILogger<LDLAppController> logger)
        {
            _logger = logger;
        }

        [HttpPost("AddNew/{PersonID}/{LicenseTypeID}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> AddNew(int PersonID,int LicenseTypeID)
        {
            _logger.LogInformation("Starting execution AddNew ldlapp ");

            try
            {
                if (PersonID < 1 || LicenseTypeID < 1)
                {
                    _logger.LogError("INVALID user Input PersonID : {@PersonID} , LicenseTypeID : {@LicenseTypeID}",
                        PersonID, LicenseTypeID);
                    return BadRequest(new { error = "Invalid User Input!!" });
                }

                _logger.LogInformation("Valid input user PersonID : {0} , LicenseTypeID : {1}", PersonID, LicenseTypeID);

                int licenseid = 0;
                if(clsLicenses.IsThisPersonHasLicenseFromThisType(PersonID,LicenseTypeID,ref licenseid))
                {
                    _logger.LogError("This Person Already has license from this Type with ID : {@Licenseid}", licenseid);
                    return BadRequest($"Person With ID : {PersonID} ,Is Already Has License From This Type With ID : {licenseid}");
                }

                int ApplicationID = 0;
                if(clsOrders.IsThisPersonIDHasAnActiveApplicationToThisLicenseTypeID(PersonID,LicenseTypeID,ref ApplicationID))
                {
                    _logger.LogError("This Person already has an active application to this License with AppID : {@ApplicationID}", ApplicationID);
                    return BadRequest($"This person is already has an open application for " +
                        $"this License type , application ID : {ApplicationID}");
                }

                clsHandleLDLApp HldlApp = new clsHandleLDLApp(PersonID,LicenseTypeID);
                if (!await HldlApp.Save())
                {
                    _logger.LogCritical("Error in saving Application or LDLApp");

                    return StatusCode(500, new { error = "Internal Server Error" });
                }

                _logger.LogInformation($"LDLApp added successfully with ID : {HldlApp.ldlApp.LDLAppID}  and  ApplicationID : {HldlApp.ldlApp.AppID}");

                return Ok(HldlApp);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Executing AddNew ldlapp in LDLAppController");
                return StatusCode(500, new { error = "Internal server Error" });
            }

        }

        [HttpGet("GetAll")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<LDLAppDataLayer.LDLAppDTO>>> GetAll()
        {
            _logger.LogInformation("Starting execute GetAll LDLApp");
            List<LDLAppDataLayer.LDLAppDTO>LDLAppDTOList=await LDLApp.GetAllLDLApps();

            if (LDLAppDTOList==null||!LDLAppDTOList.Any())
            {
                _logger.LogWarning("LDAppDTOList is empty or null");
                return NotFound("There is no LDLApps right now");
            }

            _logger.LogInformation("Number Of fetched LDLAppDTO  {@Count}", LDLAppDTOList.Count);

            return Ok(LDLAppDTOList);
        }

        [HttpGet("GetByID/{LDLAppID}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public IActionResult GetByID(int LDLAppID)
        {
            _logger.LogInformation("Starting GetByID Method (LDLAppController)");

            try
            {
                if (LDLAppID < 1)
                {
                    _logger.LogError($"Invalid User Input LDLAppID : {LDLAppID}");
                    return BadRequest("Invalid User Input");
                }

                LDLApp ldlapp = LDLApp.FindLDLApp(LDLAppID);

                if (ldlapp == null)
                {
                    _logger.LogError("ldlapp equal null");
                    return BadRequest($"There is no ldlapp with ID : {LDLAppID}");
                }

                _logger.LogInformation($"LDLApp successfully Found with ID : {ldlapp.LDLAppID} and ApplicationID : {ldlapp.AppID}");

                return Ok(ldlapp);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetByID Method (LDLAppController)");
                return StatusCode(500, new { error = "Internal server Error" });
            }
        }



    }
}
