
using StudentIMS.Models;
using System.ComponentModel.DataAnnotations;

namespace SmartCourseSelectorWeb.Models
{
    public class Course
    {
        [Key]
        public int CourseID { get; set; }
        public string CourseCode { get; set; }
        public string CourseName { get; set; }
        public bool IsMandatory { get; set; }
        public int Credit { get; set; }
        public string Department { get; set; }

        public int? Quota { get; set; } 

        public ICollection<StudentCourseSelection> StudentCourseSelections { get; set; } = new List<StudentCourseSelection>();
        public ICollection<UnapprovedSelections> UnapprovedSelections { get; set; } = new List<UnapprovedSelections>();
    }
}
