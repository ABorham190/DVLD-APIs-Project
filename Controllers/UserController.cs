using DVDLBussinessLayer;
using DVLDdataAccessLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace dvld_api.Controllers
{
    [Route("api/User")]
    [ApiController]
    public class UserController : ControllerBase
    {
        [HttpGet("GetAll")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public IActionResult GetAll()
        {
            List<clsUsersDataLayer.UserDTO> UsersList = clsUsers.GetUsersDTO();
            if (UsersList.Count > 0)
            {
                return Ok(UsersList);
            }

            return NoContent();
        }

        [HttpGet("FindByUserNameAndPassWord/{UserName}/{Password}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public ActionResult FindByUserNameAndPassword(string UserName, string Password)
        {
            if (string.IsNullOrEmpty(Password) || string.IsNullOrEmpty(UserName))
            {
                return BadRequest("Please Enter UserName and Password");

            }
            clsUsers User = clsUsers.FindUser(UserName, Password);

            if (User == null)
            {
                return BadRequest("Incorrect UserName Or Password");
            }

            return Ok(User);
        }

       
    }
}
