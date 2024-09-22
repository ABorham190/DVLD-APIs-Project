using DVDLBussinessLayer;
using DVLDdataAccessLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace dvld_api.Controllers
{
    [Route("api/LDLApp")]
    [ApiController]
    public class LDLAppController : ControllerBase
    {
        [HttpPost("AddNew")]

        public IActionResult AddNew(int PersonID,LDLApp ldlapp)
        {
            try
            {
                if (ldlapp.AppID < 1 || ldlapp.LicenseTypeID < 1 || ldlapp.LicenseTypeID > 7 || ldlapp == null)
                {
                    return BadRequest("Invalid User Input");
                }

                int LicenseID = 0;
                if(clsLicenses.IsThisPersonHasLicenseFromThisType(PersonID,ldlapp.LicenseTypeID,ref LicenseID))
                {
                    return BadRequest($"Person With ID : {PersonID} ,Is Already Has License From This Type With ID : {LicenseID}");
                }

                int ApplicationID = 0;
                if(clsOrders.IsApplicationExist(PersonID,ldlapp.LicenseTypeID,ref ApplicationID))
                {
                    return BadRequest($"This person is already has an open application for " +
                        $"this License type , application ID : {ApplicationID}");
                }

                if (!ldlapp.Save())
                {
                    return StatusCode(500, new { error = "Internal Server Error" });
                }

                return Ok(ldlapp);
            }
            catch (Exception ex)
            {
                Settings.AddErrorToEventViewer("Error : " , ex.Message);
                return StatusCode(500, new { error = "Internal server Error" });
            }

        }
    }
}
