using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using WebApi.Data;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Collections.Generic;
using System.Security.Claims;
using System;

namespace WebApi.Middleware
{
    public class EmailVerificationMiddleware
    {
        private readonly RequestDelegate _next;

        public EmailVerificationMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, AppDbContext dbContext)
        {
            // Exclude middleware for specific paths (signup and login)
            if (context.Request.Path.StartsWithSegments("/api/auth/user/register") ||
                context.Request.Path.StartsWithSegments("/api/auth/user/login") ||
                context.Request.Path.StartsWithSegments("/api/auth/student/login"))
            {
                await _next(context);
                return;
            }

            // Get the token from the request header
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null)
            {
                // Decode token and extract email
                var email = GetEmailFromToken(token);

                if (email == "500")
                {
                    // Unauthorized access
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Unauthorized access: Invalid Token");
                    return;
                }

                // Check if the student with the given email exists
                var student = dbContext.Students.SingleOrDefault(Student => Student.STUDENT_EMAIL == email);
                var user = dbContext.Users.SingleOrDefault(User => User.USER_EMAIL == email);
                if (student != null|| user != null)
                {
                    // Student found, proceed with the request
                    await _next(context);
                }
                else
                {
                    // Unauthorized access
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Unauthorized access: Token not registered with us");
                }
            }
            else
            {
                // No token provided
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized access: Token not provided");
            }
        }

        private string GetEmailFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("jL0fcjRKi3YVNYBEo2VjnDGf4k1sFpX8v2P3VKwnTVY=");
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false
            };
            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
                var jwtSecurityToken = securityToken as JwtSecurityToken;

                // Access the "Email" claim directly
                return jwtSecurityToken?.Claims.FirstOrDefault(Claim => Claim.Type == ClaimTypes.Email)?.Value;
            }
            catch (Exception ex)
            {
                // Invalid token provided
                return "500";
            }
        }
    }
}
