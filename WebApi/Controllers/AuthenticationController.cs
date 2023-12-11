using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Models;
using WebApi.Services;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthenticationController : ControllerBase
    {
        private readonly IStudentService _studentService;
        private readonly IUserService _userService;

        public AuthenticationController(IStudentService studentService, IUserService userService)
        {
            _studentService = studentService;
            _userService = userService;
        }

        [HttpPost("student/login")]
        public async Task<IActionResult> StudentLogin([FromBody] LoginRequest request)
        {
            var result = await _studentService.LoginStudent(request.Email, request.Password);

            if (result.Success)
            {
                return Ok(new { message = result.Message, token = result.Token });
            }

            return new ObjectResult(new { message = result.Message }) { StatusCode = 401 };
        }

        [HttpPost("student/register")]
        public ActionResult<bool> StudentRegister([FromBody] StudentModel inputModel)
        {
            if (inputModel == null)
            {
                return BadRequest("Invalid input data.");
            }

            bool result = _studentService.AddStudent(inputModel.ID_STUDENT_CATEGORY, inputModel.ID_ROLE, inputModel.STUDENT_EMAIL, inputModel.STUDENT_PASSWORD, inputModel.CREATED_BY_USER);

            if (result)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest("Failed to add student.");
            }
        }

        [HttpPost("user/login")]
        public async Task<IActionResult> UserLogin([FromBody] LoginRequest request)
        {
            var result = await _userService.LoginUser(request.Email, request.Password);

            if (result.Success)
            {
                return Ok(new { message = result.Message, token = result.Token });
            }

            return new ObjectResult(new { message = result.Message }) { StatusCode = 401 };
        }

        [HttpPost("user/register")]
        public ActionResult<bool> UserRegister([FromBody] UserModel inputModel)
        {
            if (inputModel == null)
            {
                return BadRequest("Invalid input data.");
            }

            bool result = _userService.AddUser(inputModel.ID_ROLE, inputModel.USER_EMAIL, inputModel.USER_PASSWORD, inputModel.CREATED_BY_USER);

            if (result)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest("Failed to add student.");
            }
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
