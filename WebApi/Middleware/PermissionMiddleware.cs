using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using WebApi.Data;
using WebApi.Models;

namespace WebApi.Middleware
{
    public class PermissionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public PermissionMiddleware(RequestDelegate next, IServiceScopeFactory serviceScopeFactory)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        }

        public async Task InvokeAsync(HttpContext context, AppDbContext dbContext)
        {
            if (context.Request.Path.StartsWithSegments("/api/auth/user/register") ||
                context.Request.Path.StartsWithSegments("/api/auth/user/login") ||
                context.Request.Path.StartsWithSegments("/api/auth/student/login"))
            {
                await _next(context);
                return;
            }
            var userClaims = context.User.Claims;
            //var roleClaim = userClaims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
            var roleIDClaim = userClaims.FirstOrDefault(c => c.Type == "roleID");

            if (roleIDClaim != null && long.TryParse(roleIDClaim.Value, out var roleID))
            {
                var permissions = await GetRolePermissionsAsync(roleID);

                // Check if the user has the necessary permission for the current request
                if (CheckPermission(context.Request.Method, permissions))
                {
                    // User has the necessary permission, proceed with the request
                    await _next(context);
                    return;
                }

                context.Response.StatusCode = 403; // Forbidden
                await context.Response.WriteAsync("Unauthorized access: Not Permitted");
                return;
            }

            context.Response.StatusCode = 401; // Unauthorized
            await context.Response.WriteAsync("Unauthorized access: Invalid Token");
        }

        private async Task<string[]> GetRolePermissionsAsync(long roleId)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var permissionIds = await dbContext.RolePermissions
                    .Where(rp => rp.ID_ROLE == roleId)
                    .Select(rp => rp.ID_PERMISSION)
                    .ToArrayAsync();

                var permissions = await dbContext.Permissions
                    .Where(p => permissionIds.Contains(p.ID_PERMISSION))
                    .Select(p => p.PERMISSION)
                    .ToArrayAsync();

                return permissions;
            }
        }

        private static bool CheckPermission(string method, string[] permissions)
        {
            // Your logic to check if the user has the necessary permission based on the request method
            // Example logic: Check if the requested method is allowed for the given permissions
            var operation = GetOperationForMethod(method);

            return permissions.Any(p => p.Equals(operation, StringComparison.OrdinalIgnoreCase));
        }

        private static string GetOperationForMethod(string method)
        {
            switch (method.ToUpper())
            {
                case "GET":
                    return "Read";
                case "POST":
                    return "Create";
                case "PUT":
                    return "Update";
                case "DELETE":
                    return "Delete";
                default:
                    return string.Empty;
            }
        }
    }

    public static class PermissionMiddlewareExtensions
    {
        public static IApplicationBuilder UsePermissionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<PermissionMiddleware>();
        }
    }
}
