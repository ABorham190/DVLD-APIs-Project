using DVDLBussinessLayer;
using DVLDdataAccessLayer;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using Microsoft.Data.SqlClient;
using System.Reflection.Metadata.Ecma335;

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

        [HttpGet("FindByUserID/{UserID}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]

        public IActionResult FindByUserID(int UserID)
        {
            if (UserID < 0) { return BadRequest("Invalid input parameters"); }

            clsUsers User=clsUsers.FindUser(UserID);

            if (User == null) {

                return BadRequest($"There is no User with ID : {UserID}");
            }

            clsUsersDataLayer.UserDTO userdto = new clsUsersDataLayer.UserDTO(User.UserID,User.Person.FullName,
                User.IsActive,User.UserName);

            return Ok(userdto);
        }

        [HttpGet("GetUserNameByUserID/{UserID}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public IActionResult GetUserNameByUserID(int UserID)
        {
            if (UserID<1)
            {
                return BadRequest("Invalid input parameters !!");
            }
            string UserName = null;
            if (!clsUsers.GetUserNameByUserID(UserID,ref UserName))
            {
                return NotFound();
            }

            return Ok($"UserName for This userID Is : {UserName}");
        }

        [HttpPost("AddNew")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public ActionResult AddNew(clsUsersDataLayer.AddNewUserDTO adduserdto)
        {
            if (string.IsNullOrEmpty(adduserdto.UserName) || string.IsNullOrEmpty(adduserdto.Password)
                || adduserdto.PersonID < 1)
            {
                return BadRequest("Invalid User Info");
            }
            clsUsers User = new clsUsers();

            try
            {
                
                User.PersonID = adduserdto.PersonID;
                User.UserName = adduserdto.UserName;
                User.UserPassword = adduserdto.Password;
                User.IsActive = adduserdto.IsActive;

                if ((User.Person = clsPerson.FindPerson(User.PersonID))==null)
                {
                    return BadRequest($"Invalid User Info (There is no person with ID : {adduserdto.PersonID})");
                }


                if (clsPerson.IsPersonAUser(adduserdto.PersonID))
                {
                    return BadRequest("Invalid Input parameters ,this person is already USER !!");
                }
                if (!User.Save())
                {
                    return StatusCode(500, new { error = "Internal server error" });
                }

               
            }
            catch
            {
                return StatusCode(500, new { error = "Internal server error" });
            }

            return Ok($"User successfully added with ID : {User.UserID}");
        }

        [HttpDelete("Delete/{UserID}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public ActionResult Delete(int UserID)
        {
            if(UserID < 1)
            {
                return BadRequest("Invalid input parameter !!");
            }

            if (!clsUsers.IsUserExists(UserID))
            {
                return BadRequest($"There is no user with ID : {UserID}");
            }

            if (!clsUsers.DeleteUser(UserID))
            {
                return StatusCode(500, new { error = "internal server error (cant delete this user)" });
            }

            return Ok("User Deleted Successfully");
        }

        [HttpPatch("MakeUserActiveOrInActive")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public IActionResult MakeUserActiveOrInActive(int userID,bool isactive)
        {
            if (userID < 1) {

                return BadRequest("Invalid Input Parameter !!");
            }
            clsUsers user = new clsUsers();

            if((user = clsUsers.FindUser(userID)) == null)
            {
                return BadRequest($"There is no user with ID : {userID}");
            }

            if (user.IsActive == isactive)
            {
                return user.IsActive ? Ok("This User is already ACTIVE") : Ok("This User is already INACTIVE");
            }
            else
            {
                user.IsActive = isactive;
            }

            if (!user.Save())
            {
                return StatusCode(500, new { error = "Internal server error" });
            }

            clsUsersDataLayer.UserDTO userdto = new clsUsersDataLayer.UserDTO(user.UserID,user.Person.FullName,user.IsActive,user.UserName);

            return Ok(userdto);

        }

        [HttpPut("Update/{UserID}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]


        public IActionResult Update(clsUsersDataLayer.UpdateUserDTO user,int UserID)
        {
            if (UserID < 1||string.IsNullOrEmpty(user.UserName)||string.IsNullOrEmpty(user.Password)) 
            { 
                return BadRequest("Invalid input parameter");
            }

            clsUsers User = clsUsers.FindUser(UserID);
            if(User == null)
            {
                return BadRequest($"There is no User with ID : {UserID}");
            }

            User.UserName = user.UserName;
            User.UserPassword = user.Password;
            User.IsActive = user.IsActive;

            if (!User.Save())
            {
                return StatusCode(500, new { error = "Internal server Error" });
            }

            return Ok(user);

        }



    }
}
