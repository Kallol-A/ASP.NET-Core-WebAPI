using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using WebApi.Data;
using WebApi.Models;

namespace WebApi.Services
{
    public interface IRoleService
    {
        bool AddRole(string role, string createdBy);

        IEnumerable<RoleModel> GetAllRoles();
    }

    public class RoleService : IRoleService
    {
        private readonly AppDbContext _dbContext;

        // Constructor
        public RoleService(AppDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public bool AddRole(string role, string createdBy)
        {
            try
            {
                var _role = new RoleModel
                {
                    ROLE = role,
                    CREATED_BY_USER = createdBy,
                    CREATED_AT = DateTime.Now
                };

                _dbContext.Roles.Add(_role);
                _dbContext.SaveChanges();

                return true; // Operation succeeded
            }
            catch (Exception ex)
            {
                // Log the exception

                return false; // Operation failed
            }
        }

        public IEnumerable<RoleModel> GetAllRoles()
        {
            return _dbContext.Roles.ToList();
        }
    }
}
