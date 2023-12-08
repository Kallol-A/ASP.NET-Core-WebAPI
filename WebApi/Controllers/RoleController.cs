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
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;

        public RoleController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        // GET api/role
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            var roles = _roleService.GetAllRoles();
            return Ok(roles);
        }

        // POST api/role
        [HttpPost]
        public ActionResult<bool> Post([FromBody] RoleModel inputModel)
        {
            if (inputModel == null)
            {
                return BadRequest("Invalid input data.");
            }

            bool result = _roleService.AddRole(inputModel.ROLE, inputModel.CREATED_BY_USER);

            if (result)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest("Failed to add role.");
            }
        }
    }
}
