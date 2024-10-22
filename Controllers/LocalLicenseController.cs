using AutoMapper;
using DVDLBussinessLayer;
using dvld_api.models;
using DVLDdataAccessLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace dvld_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocalLicenseController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly ILogger<LocalLicenseController> _logger;
        public LocalLicenseController(IMapper mapper, ILogger<LocalLicenseController> logger)
        {
            _mapper = mapper;
            _logger = logger;
        }

        [HttpPost("AddNew/{LDLAppID}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult>AddNew(int LDLAppID)
        {
            _logger.LogInformation("Start execution AddNew method in LocalLicenseController");
            clsLicenses license = new clsLicenses();
            clsDriver driver = new clsDriver();
            _logger.LogInformation("Creating instances from clsLiceses and clsDriver classes");
            if (LDLAppID < 1)
            {
                _logger.LogInformation($"Invalid User input LDLApp : {LDLAppID}");
                return BadRequest($"Invalid User Input LDLAppID : {LDLAppID}");
            }

            try { 

                LDLApp ldlApp = LDLApp.FindLDLApp(LDLAppID);
                if (ldlApp == null)
                {
                    _logger.LogError($"There is no LDLApp with this LDLAppID : {LDLAppID}");
                    return NotFound($"There is no application with AppID : {LDLAppID}"); 
                }

                int DriverID = 0;

                if (!clsPerson.IsThisPersonADriver(ldlApp.Application.ApplicantID, ref DriverID))
                {
                    _logger.LogInformation("Applicant person is not a driver");
                    HandleIssueLicense.FillDriverObject(driver, ldlApp);

                    if (driver.AddNewDriver())
                    {
                        _logger.LogInformation($"Driver added successfully with ID : {driver.DriverID}");
                        license.DriverID = driver.DriverID;

                        await HandleIssueLicense.FillLicenseObject(license, ldlApp);
                        

                        if (license.Save())
                        {
                            _logger.LogInformation($"License Added Successfully with ID : {license.LicenseID}");
                            Console.WriteLine($"License saved successfully: {JsonConvert.SerializeObject(license)}");
                            await clsOrders.UpdateApplicationStatus(ldlApp.AppID, clsOrdersDataLayer.enWhatToDo.Complete);
                            _logger.LogInformation($"Application updated successfully to {ldlApp.Application.ApplicationStatus}");

                            LicenseDTO licenseDTO=_mapper.Map<LicenseDTO>(license);

                            if (licenseDTO != null)
                            {
                                _logger.LogInformation("AddNew method in LocalLicenseController Executed successfully");

                                return Ok(licenseDTO);
                            }
                            else
                            {
                                _logger.LogError("licenseDTO is null");
                                return StatusCode(500, "Internal server error");
                            }
                        }
                    }
                }
                else
                {
                    _logger.LogInformation($"Person is already driver with driverID : {DriverID}");
                    license.DriverID = DriverID;

                    await HandleIssueLicense.FillLicenseObject(license, ldlApp);

                    if (license.Save())
                    {
                        _logger.LogInformation($"License Added Successfully with ID : {license.LicenseID}");

                        Console.WriteLine($"License saved successfully: {JsonConvert.SerializeObject(license)}");
                        await clsOrders.UpdateApplicationStatus(ldlApp.AppID, clsOrdersDataLayer.enWhatToDo.Complete);
                        _logger.LogInformation($"Application updated successfully to {ldlApp.Application.ApplicationStatus}");

                        LicenseDTO licenseDTO = _mapper.Map<LicenseDTO>(license);

                        if (licenseDTO != null)
                        {
                            _logger.LogInformation("AddNew method in LocalLicenseController Executed successfully");

                            return Ok(licenseDTO);
                        }
                        else
                        {
                            _logger.LogError("licenseDTO is null");
                            return StatusCode(500, "Internal server error");
                        }

                    }
                }
                return StatusCode(500, "Internal server Error");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "Internal server Error");
            }
        }

        [HttpGet("GetByID")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> GetByID(int LicenseID)
        {
            _logger.LogInformation("Start Executing GetByID Method in LocalLicenseController");

            if (LicenseID < 1)
            {
                _logger.LogError($"Invalid User Input {LicenseID}");
                return BadRequest($"Invalid User Input {LicenseID}");
            }

            try
            {
                var license = clsLicenses.GetLicenseDetailsByLLicenseID(LicenseID);
                if (license == null)
                {
                    _logger.LogError($"There is no Licesne with ID : {LicenseID}");
                    return NotFound($"There is no Licesne with ID : {LicenseID}");
                }

                LicenseDTO licenseDTO = _mapper.Map<LicenseDTO>(license);
                if (licenseDTO != null) {
                    _logger.LogInformation($"license Found Successfully,{licenseDTO}");
                    return Ok(licenseDTO);
                }
                else
                {
                    _logger.LogError("licenseDTO is null here");
                    return StatusCode(500, "Internal Server Error");
                }

            }catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(500, "Internal server error");
            }
        }

    }
}
