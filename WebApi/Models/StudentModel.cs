using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;

namespace WebApi.Models
{
    public class StudentModel
    {
        [Key]
        public long ID_STUDENT { get; set; }

        public long ID_STUDENT_CATEGORY { get; set; }
        public long ID_ROLE { get; set; }
        public string STUDENT_EMAIL { get; set; }
        public string STUDENT_PASSWORD { get; set; }
        public string CREATED_BY_USER { get; set; }
        public string LAST_UPDATED_BY_USER { get; set; }
        public string DELETED_BY_USER { get; set; }
        public DateTime? CREATED_AT { get; set; }
        public DateTime? UPDATED_AT { get; set; }
        public DateTime? DELETED_AT { get; set; }
    }
}
