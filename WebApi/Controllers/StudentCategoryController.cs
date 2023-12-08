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
    public class StudentCategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public StudentCategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        // GET api/studentcategory
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            var categories = _categoryService.GetAllCategories();
            return Ok(categories);
        }

        // POST api/studentcategory
        [HttpPost]
        public ActionResult<bool> Post([FromBody] StudentCategoryModel inputModel)
        {
            if (inputModel == null)
            {
                return BadRequest("Invalid input data.");
            }

            bool result = _categoryService.AddStudentCategory(inputModel.STUDENT_CATEGORY, inputModel.CREATED_BY_USER);

            if (result)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest("Failed to add student category.");
            }
        }

        // PUT api/studentcategory
        [HttpPut]
        public ActionResult<bool> Put([FromBody] StudentCategoryModel updateModel)
        {
            if (updateModel == null || updateModel.ID_STUDENT_CATEGORY <= 0)
            {
                return BadRequest("Invalid input data or missing ID.");
            }

            bool result = _categoryService.UpdateStudentCategory(updateModel.ID_STUDENT_CATEGORY, updateModel.STUDENT_CATEGORY, updateModel.LAST_UPDATED_BY_USER);

            if (result)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest($"Failed to update student category with ID {updateModel.ID_STUDENT_CATEGORY}.");
            }
        }

        // DELETE api/studentcategory/{studentId}
        [HttpDelete("{studentId}")]
        public ActionResult<bool> Delete(long studentId, [FromBody] StudentCategoryModel deleteModel)
        {
            if (studentId <= 0 || string.IsNullOrEmpty(deleteModel.DELETED_BY_USER))
            {
                return BadRequest("Invalid ID.");
            }

            bool result = _categoryService.DeleteStudentCategory(studentId, deleteModel.DELETED_BY_USER);

            if (result)
            {
                return Ok(result);
            }
            else
            {
                return NotFound($"Student category with ID {studentId} not found or failed to delete.");
            }
        }
    }
}