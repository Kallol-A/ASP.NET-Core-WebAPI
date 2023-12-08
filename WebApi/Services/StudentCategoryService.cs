using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using WebApi.Data;
using WebApi.Models;

namespace WebApi.Services
{
    public interface ICategoryService
    {
        bool AddStudentCategory(string studentCategory, string createdBy);
        bool UpdateStudentCategory(long studentCategoryId, string studentCategory, string createdBy);
        bool DeleteStudentCategory(long studentCategoryId, string deletedBy);
        IEnumerable<StudentCategoryModel> GetAllCategories();
    }

    public class StudentCategoryService : ICategoryService
    {
        private readonly AppDbContext _dbContext;

        // Constructor
        public StudentCategoryService(AppDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }

        public bool AddStudentCategory(string studentCategory, string createdBy)
        {
            try
            {
                var _category = new StudentCategoryModel
                {
                    STUDENT_CATEGORY = studentCategory,
                    CREATED_BY_USER = createdBy,
                    CREATED_AT = DateTime.Now
                };

                _dbContext.StudentCategories.Add(_category);
                _dbContext.SaveChanges();

                return true; // Operation succeeded
            }
            catch (Exception ex)
            {
                // Log the exception

                return false; // Operation failed
            }
        }

        public bool UpdateStudentCategory(long studentCategoryId, string StudentCategory, string updatedBy)
        {
            try
            {
                // Find the existing student category by ID
                var existingCategory = _dbContext.StudentCategories.Find(studentCategoryId);

                if (existingCategory != null)
                {
                    // Update the properties
                    existingCategory.STUDENT_CATEGORY = StudentCategory;
                    existingCategory.LAST_UPDATED_BY_USER = updatedBy;
                    existingCategory.UPDATED_AT = DateTime.Now;

                    // Save changes
                    _dbContext.SaveChanges();

                    return true; // Operation succeeded
                }
                else
                {
                    // Student category not found
                    return false; // Operation failed
                }
            }
            catch (Exception ex)
            {
                // Log the exception

                return false; // Operation failed
            }
        }

        public IEnumerable<StudentCategoryModel> GetAllCategories()
        {
            return _dbContext.StudentCategories
                .Where(category => category.DELETED_AT == null)
                .ToList();
        }

        public bool DeleteStudentCategory(long studentCategoryId, string deletedBy)
        {
            try
            {
                var categoryToDelete = _dbContext.StudentCategories.Find(studentCategoryId);

                if (categoryToDelete != null)
                {
                    //_dbContext.StudentCategories.Remove(categoryToDelete);
                    categoryToDelete.DELETED_BY_USER = deletedBy;
                    categoryToDelete.DELETED_AT = DateTime.Now;

                    _dbContext.SaveChanges();

                    return true; // Deletion succeeded
                }
                else
                {
                    return false; // Category not found
                }
            }
            catch (Exception ex)
            {
                // Log the exception
                return false; // Deletion failed
            }
        }
    }
}
