﻿using System;
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

        IEnumerable<StudentModel> GetAllStudents();
    }

    public class StudentService : IStudentService
    {
        private readonly AppDbContext _dbContext;
        private readonly IPasswordHasherService _passwordHasherService;

        // Constructor
        public StudentService(AppDbContext dbContext, IPasswordHasherService passwordHasherService)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _passwordHasherService = passwordHasherService ?? throw new ArgumentNullException(nameof(passwordHasherService));
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
            // Fetch the user information, including the role, from the database
            var student = _dbContext.Students.FirstOrDefault(s => s.STUDENT_EMAIL == studentEmail && s.DELETED_AT == null);

            if (student == null)
            {
                // Handle the case where the user is not found
                // You might want to log an error or throw an exception
                return null;
            }

            // Retrieve the user's role from the database
            var role = _dbContext.Roles.FirstOrDefault(r => r.ID_ROLE == student.ID_ROLE);

            if (role == null)
            {
                // Handle the case where the role is not found
                // You might want to log an error or throw an exception
                return null;
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, studentEmail),
                new Claim(ClaimTypes.Role, role.ROLE),
                // Add more claims as needed
                new Claim("roleID", role.ID_ROLE.ToString()),
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

            return accessToken;
        }
    }

    public class LoginResult
    {
        public bool Success { get; set; }
        public string Token { get; set; }
        public string Message { get; set; }
    }
}
