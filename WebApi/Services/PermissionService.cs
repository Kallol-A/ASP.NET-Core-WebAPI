using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using WebApi.Data;
using WebApi.Models;

namespace WebApi.Services
{
    public interface IPermissionService
    {
        bool AddPermission(long roleID, string Permission, string createdBy);

        IEnumerable<PermissionModel> GetAllPermissions();
    }

    public class PermissionService : IPermissionService
    {
        private readonly AppDbContext _dbContext;

        // Constructor
        public PermissionService(AppDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public bool AddPermission(long roleID, string Permission, string createdBy)
        {
            try
            {
                var _permission = new PermissionModel
                {
                    ID_ROLE = roleID,
                    PERMISSION = Permission,
                    CREATED_BY_USER = createdBy,
                    CREATED_AT = DateTime.Now
                };

                _dbContext.Permissions.Add(_permission);
                _dbContext.SaveChanges();

                return true; // Operation succeeded
            }
            catch (Exception ex)
            {
                // Log the exception

                return false; // Operation failed
            }
        }

        public IEnumerable<PermissionModel> GetAllPermissions()
        {
            return _dbContext.Permissions.ToList();
        }
    }
}
