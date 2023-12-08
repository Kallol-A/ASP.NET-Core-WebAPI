using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using WebApi.Data;
using WebApi.Models;

namespace WebApi.Services
{
    public interface IStudentService
    {
        bool AddStudent(long studentCategoryID, long roleID, string studentEmail, string studentPassword, string createdBy);
        //bool UpdateStudent(long studentCategoryId, string studentCategory, string createdBy);
        //bool DeleteStudent(long studentCategoryId, string deletedBy);
        IEnumerable<StudentModel> GetAllStudents();
    }

    public class StudentService : IStudentService
    {
        private readonly AppDbContext _dbContext;
        private readonly IPasswordHasherService _passwordHasherService;

        // Constructor
        public StudentService(AppDbContext dbContext,IPasswordHasherService passwordHasherService)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _passwordHasherService = passwordHasherService;
        }

        public bool AddStudent(long studentCategoryID, long roleID, string studentEmail, string studentPassword, string createdBy)
        {
            try
            {
                var _student = new StudentModel
                {
                    ID_STUDENT_CATEGORY = studentCategoryID,
                    ID_ROLE = roleID,
                    STUDENT_EMAIL = studentEmail,
                    STUDENT_PASSWORD = _passwordHasherService.HashPassword(studentPassword),
                    CREATED_BY_USER = createdBy,
                    CREATED_AT = DateTime.Now
                };

                _dbContext.Students.Add(_student);
                _dbContext.SaveChanges();

                return true; // Operation succeeded
            }
            catch (Exception ex)
            {
                // Log the exception

                return false; // Operation failed
            }
        }

        public IEnumerable<StudentModel> GetAllStudents()
        {
            return _dbContext.Students
                .Where(student => student.DELETED_AT == null)
                .ToList();
        }
    }
}
