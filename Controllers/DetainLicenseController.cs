﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using dvld_api.models;
using AutoMapper;
using DVDLBussinessLayer;

namespace dvld_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetainLicenseController : ControllerBase
    {
        private readonly ILogger<DetainLicenseController> _logger;
        private readonly IMapper _mapper;

        public DetainLicenseController(ILogger<DetainLicenseController> logger, IMapper mapper)
        {
            _logger = logger;
            _mapper = mapper;
        }

        [HttpPost("AddNew")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddNew(DetainLicenseDTO detainLicenseDTO)
        {
            _logger.LogInformation("Start executing AddNew DetainLicenseController");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var NewDetainID = HandleAddNewDetain.AddNewDetain(detainLicenseDTO);

                if (await NewDetainID != -1)
                {
                    _logger.LogInformation($"Detain added successfully with ID : {NewDetainID}");
                    return Ok(NewDetainID);
                }
                else
                {
                    _logger.LogError($"Error in saving new detain");
                    return StatusCode(500, "Internal server error");
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UnExcepected Error " + ex.Message);
                var response = new
                {
                    error = "Internal server Error",
                    code = 500
                };
                return StatusCode(500, response);
            }
        }

        [HttpGet("GetAll")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<IActionResult> GetAll()
        {
            _logger.LogInformation("Start executing GetAll DetainedLicenseController");
            try
            {
                var detainedLicenses = await clsDetain.GetAllDetainedLicenses();
                if (!detainedLicenses.Any())
                {
                    _logger.LogWarning("No detained licenses found");
                    return NotFound("No detained licenses found");
                }
                _logger.LogInformation($"Number of All detained license : {detainedLicenses.Count}");
                return Ok(detainedLicenses);
            }
            catch (Exception ex)
            {
                _logger.LogError("Unexcepected error occurs");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpPatch("ReleaseDetainedLicense")]
        public async Task<IActionResult> ReleaseDetianedLicense(int LicenseID, int CreatedByUserID)
        {
            _logger.LogInformation("Start executing ReleaseDetainedLicense DetainLicense controller");

            if (LicenseID < 1 || CreatedByUserID < 1)
            {
                _logger.LogError($"Invalid User Input LicenseID {LicenseID} , CreatedByUserID {CreatedByUserID}");
                return BadRequest($"Invalid User Input LicenseID {LicenseID} , CreatedByUserID {CreatedByUserID}");
            }
            try
            {
                if (await clsDetain.ReleaseDetainedLicense(LicenseID, CreatedByUserID))
                {
                    _logger.LogInformation("License Released successfully");
                    return Ok("License Released successfully");
                }
                _logger.LogError("License not Released successfully");
                return StatusCode(500, "Internal  server error");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, "Error in ReleaseDetainedLicense method ");
                return StatusCode(500, "Internal  server error");
            }
        }
    }
}
