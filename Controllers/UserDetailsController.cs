using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Immutable;
using web_rest.Dto;
using web_rest.Models;
using web_rest.Services;

namespace web_rest.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Produces("application/json")]
    public class UserDetailsController(
        IUserService userService, 
        ILogger<UserDetailsController> logger
    ): ControllerBase
    {

        [HttpGet]
        [ResponseCache(Duration = 60)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<UserDetails>>> GetAll() =>
            Ok(await userService.GetUsersAsync());

        [HttpGet("email/{email}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDetails>> GetByEmail(string email)
        {
            var user = await userService.FindByEmailAsync(email);
            return user == null
                ? StatusCode(StatusCodes.Status404NotFound, $"User by the email {email} does noyt exists")
                : StatusCode(StatusCodes.Status200OK, user);
        }

        [HttpGet("email")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<UserDetails?>>> FilterByEmail([FromQuery] string emailFirstLetters)
        {
            var user = await userService.FindByEmailFirstLettersAsync(emailFirstLetters);
            return user != null
                ? new ApiResponse<UserDetails?>()
                    {
                        Data = user,
                        Success = true,
                        StatusCode = StatusCodes.Status200OK
                    }
                : new ApiResponse<UserDetails?>()
                    {
                        Success = false,
                        StatusCode = StatusCodes.Status404NotFound,
                        Message = $"Could not find a user according to {emailFirstLetters}"
                    };
        }


        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserDetails>> GetById(int id)
        {
            var user = await userService.FindByIdAsync(id);
            return user == null
                ? NotFound($"User by the id {id} doesnt exists")
                : Ok(user);
        }

        [HttpPost("create")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<UserDetails>> CreateUser([FromBody] UserDetails user)
        {
            var savedUser = await userService.SaveUserAsync(user);
            return Ok(savedUser);
        }

        [HttpPut("update")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserDetails>> UpdateUser([FromBody] UserDetails user)
        {
            try
            {
                var updatedUser = await userService.UpdateUserAsync(user);
                return Ok(updatedUser);
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to update user by the id {user.Id}");
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("delete/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<UserDetails>> DeleteUser(int id)
        {
            try
            {
                var deletedUser = await userService.DeleteUserByIdAsync(id);
                return Ok(deletedUser);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("authenticate")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> Authenticate(

            [FromQuery] string email,
            [FromQuery] string password
        )
        {
            ImmutableList<string> emailAndPassword = [email, password];
            if (emailAndPassword.Any(string.IsNullOrEmpty))
            {
                return BadRequest(new { error = "Email or password is invalid" });
            }
            var user = await userService.FindByEmailAndPasswordAsync(email, password);
            if (user == null)
            {
                return Unauthorized(new { error = "User doesnt exist by this email / password" });
            }
            return Ok(new { message = "Authenticated" });
        }
            
    }
}
