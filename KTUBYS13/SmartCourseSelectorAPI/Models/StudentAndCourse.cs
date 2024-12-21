using SmartCourseSelectorWeb.Models;

namespace StudentIMS.Models
{
    public class StudentAndCourse
    {
        public Student Student { get; set; }
        public List<Course> Course { get; set; }  
    }
}
