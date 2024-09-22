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
        [HttpPost("AddNew/{PersonID}/{LicenseTypeID}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public IActionResult AddNew(int PersonID,int LicenseTypeID)
        {
            try
            {
                if (PersonID < 1 || LicenseTypeID < 1)
                {
                    return BadRequest(new { error = "Invalid User Input!!" });
                }

                int licensetypeid = 0;
                if(clsLicenses.IsThisPersonHasLicenseFromThisType(PersonID,LicenseTypeID,ref licensetypeid))
                {
                    return BadRequest($"Person With ID : {PersonID} ,Is Already Has License From This Type With ID : {licensetypeid}");
                }

                int ApplicationID = 0;
                if(clsOrders.IsThisPersonIDHasAnActiveApplicationToThisLicenseTypeID(PersonID,LicenseTypeID,ref ApplicationID))
                {
                    return BadRequest($"This person is already has an open application for " +
                        $"this License type , application ID : {ApplicationID}");
                }

                clsHandleLDLApp HldlApp = new clsHandleLDLApp(PersonID,LicenseTypeID);
                if (!HldlApp.Save())
                {
                    return StatusCode(500, new { error = "Internal Server Error" });
                }

                return Ok(HldlApp);
            }
            catch (Exception ex)
            {
                Settings.AddErrorToEventViewer("Error : " , ex.Message);
                return StatusCode(500, new { error = "Internal server Error" });
            }

        }

        [HttpGet("GetAll")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<List<LDLAppDataLayer.LDLAppDTO>>> GetAll()
        {
            List<LDLAppDataLayer.LDLAppDTO>LDLAppDTOList=await LDLApp.GetAllLDLApps();

            if (LDLAppDTOList==null||!LDLAppDTOList.Any())
            {
                return NotFound("There is no LDLApps right now");
            }

            return Ok(LDLAppDTOList);
        }



    }
}
