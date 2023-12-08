using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using WebApi.Data;
using WebApi.Models;

namespace WebApi.Services
{
    public interface IStudentService
    {
        bool AddStudent(long studentCategoryID, long roleID, string studentEmail, string studentPassword, string createdBy);

        Task<LoginResult> LoginStudent(string studentEmail, string studentPassword);
        void SetAccessTokenInSession(string accessToken);

        IEnumerable<StudentModel> GetAllStudents();
    }

    public class StudentService : IStudentService
    {
        private readonly AppDbContext _dbContext;
        private readonly IPasswordHasherService _passwordHasherService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        // Constructor
        public StudentService(AppDbContext dbContext, IPasswordHasherService passwordHasherService, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _passwordHasherService = passwordHasherService ?? throw new ArgumentNullException(nameof(passwordHasherService));
            _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        }

        public bool AddStudent(long studentCategoryID, long roleID, string studentEmail, string studentPassword, string createdBy)
        {
            try
            {
                var _student = new StudentModel
                {
                    ID_STUDENT_CATEGORY = studentCategoryID,
                    ID_ROLE = roleID,
                    STUDENT_EMAIL = studentEmail,
                    STUDENT_PASSWORD = _passwordHasherService.HashPassword(studentPassword),
                    CREATED_BY_USER = createdBy,
                    CREATED_AT = DateTime.Now
                };

                _dbContext.Students.Add(_student);
                _dbContext.SaveChanges();

                return true; // Operation succeeded
            }
            catch (Exception ex)
            {
                // Log the exception

                return false; // Operation failed
            }
        }

        public IEnumerable<StudentModel> GetAllStudents()
        {
            return _dbContext.Students
                .Where(student => student.DELETED_AT == null)
                .ToList();
        }

        public async Task<LoginResult> LoginStudent(string studentEmail, string studentPassword)
        {
            var student = _dbContext.Students
                .FirstOrDefault(Student => Student.STUDENT_EMAIL == studentEmail && Student.DELETED_AT == null);

            if (student != null && _passwordHasherService.VerifyPassword(student.STUDENT_PASSWORD, studentPassword))
            {
                var token = GenerateJwtToken(studentEmail);
                return new LoginResult { Success = true, Token = token, Message = "Login successful" };
            }

            return new LoginResult { Success = false, Token = null, Message = "Invalid email or password" };
        }

        private string GenerateJwtToken(string studentEmail)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, studentEmail),
                new Claim(ClaimTypes.Role, "Student")
                // Add more claims as needed
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("jL0fcjRKi3YVNYBEo2VjnDGf4k1sFpX8v2P3VKwnTVY="));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "studentManager",
                audience: "stuman.com",
                claims: claims,
                expires: DateTime.Now.AddHours(24), // Token expiration time
                signingCredentials: creds
            );

            var accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            return accessToken ;
        }

        public void SetAccessTokenInSession(string accessToken)
        {
            _httpContextAccessor.HttpContext.Session.SetString("AccessToken", accessToken);
        }
    }

    public class LoginResult
    {
        public bool Success { get; set; }
        public string Token { get; set; }
        public string Message { get; set; }
    }
}
