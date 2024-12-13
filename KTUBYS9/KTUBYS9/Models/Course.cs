using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace KTUBYS9.Models
{
    public class Course
    {

        public int CourseID { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public bool IsMandatory { get; set; }
        public int Credit { get; set; }
        public string Department { get; set; }
        public ICollection<StudentCourseSelection> StudentCourseSelections { get; set; } = new List<StudentCourseSelection>();

    }
}