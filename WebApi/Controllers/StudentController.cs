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
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        // GET api/student
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            var students = _studentService.GetAllStudents();
            return Ok(students);
        }

        // POST api/studentcategory
        [HttpPost]
        public ActionResult<bool> Post([FromBody] StudentModel inputModel)
        {
            if (inputModel == null)
            {
                return BadRequest("Invalid input data.");
            }

            bool result = _studentService.AddStudent(inputModel.ID_STUDENT_CATEGORY, inputModel.ID_ROLE, inputModel.STUDENT_EMAIL, inputModel.STUDENT_PASSWORD, inputModel.CREATED_BY_USER);

            if (result)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest("Failed to add student.");
            }
        }
    }
}
