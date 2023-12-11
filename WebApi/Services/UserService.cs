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
    public interface IUserService
    {
        bool AddUser(long roleID, string userEmail, string userPassword, string createdBy);

        Task<LoginResult> LoginUser(string userEmail, string userPassword);

        IEnumerable<UserModel> GetAllUsers();
    }

    public class UserService : IUserService
    {
        private readonly AppDbContext _dbContext;
        private readonly IPasswordHasherService _passwordHasherService;

        // Constructor
        public UserService(AppDbContext dbContext, IPasswordHasherService passwordHasherService)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _passwordHasherService = passwordHasherService ?? throw new ArgumentNullException(nameof(passwordHasherService));
        }

        public bool AddUser(long roleID, string userEmail, string userPassword, string createdBy)
        {
            try
            {
                var _user = new UserModel
                {
                    ID_ROLE = roleID,
                    USER_EMAIL = userEmail,
                    USER_PASSWORD = _passwordHasherService.HashPassword(userPassword),
                    CREATED_BY_USER = createdBy,
                    CREATED_AT = DateTime.Now
                };

                _dbContext.Users.Add(_user);
                _dbContext.SaveChanges();

                return true; // Operation succeeded
            }
            catch (Exception ex)
            {
                // Log the exception

                return false; // Operation failed
            }
        }

        public IEnumerable<UserModel> GetAllUsers()
        {
            return _dbContext.Users
                .Where(user => user.DELETED_AT == null)
                .ToList();
        }

        public async Task<LoginResult> LoginUser(string userEmail, string userPassword)
        {
            var user = _dbContext.Users
                .FirstOrDefault(User => User.USER_EMAIL == userEmail && User.DELETED_AT == null);

            if (user != null && _passwordHasherService.VerifyPassword(user.USER_PASSWORD, userPassword))
            {
                var token = GenerateJwtToken(userEmail);
                return new LoginResult { Success = true, Token = token, Message = "Login successful" };
            }

            return new LoginResult { Success = false, Token = null, Message = "Invalid email or password" };
        }

        private string GenerateJwtToken(string userEmail)
        {
            // Fetch the user information, including the role, from the database
            var user = _dbContext.Users.FirstOrDefault(u => u.USER_EMAIL == userEmail && u.DELETED_AT == null);

            if (user == null)
            {
                // Handle the case where the user is not found
                // You might want to log an error or throw an exception
                return null;
            }

            // Retrieve the user's role from the database
            var role = _dbContext.Roles.FirstOrDefault(r => r.ID_ROLE == user.ID_ROLE);

            if (role == null)
            {
                // Handle the case where the role is not found
                // You might want to log an error or throw an exception
                return null;
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, userEmail),
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
}
