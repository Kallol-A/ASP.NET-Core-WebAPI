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
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionController : ControllerBase
    {
        private readonly IPermissionService _permissionService;

        public PermissionController(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        // GET api/permission
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            var permissions = _permissionService.GetAllPermissions();
            return Ok(permissions);
        }

        // POST api/permission
        [HttpPost]
        public ActionResult<bool> Post([FromBody] PermissionModel inputModel)
        {
            if (inputModel == null)
            {
                return BadRequest("Invalid input data.");
            }

            bool result = _permissionService.AddPermission(inputModel.ID_ROLE, inputModel.PERMISSION, inputModel.CREATED_BY_USER);

            if (result)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest("Failed to add permission.");
            }
        }
    }
}
