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
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthenticationController(IStudentService studentService, IHttpContextAccessor httpContextAccessor)
        {
            _studentService = studentService;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await _studentService.LoginStudent(request.Email, request.Password);

            if (result.Success)
            {
                _studentService.SetAccessTokenInSession(result.Token);

                // Log or debug to check if the token is saved in the session
                var accessTokenFromSession = _httpContextAccessor.HttpContext.Session.GetString("AccessToken");
                Console.WriteLine($"Access token in session: {accessTokenFromSession}");

                return Ok(new { message = result.Message, token = result.Token });
            }

            return new ObjectResult(new { message = result.Message }) { StatusCode = 401 };
        }
    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
