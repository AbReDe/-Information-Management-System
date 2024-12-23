using SmartCourseSelectorWeb.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentIMS.Models
{
    public class UnapprovedSelections
    {
        [Key]

        public int ID { get; set; }
        
        public int CourseID { get; set; }
        public int StudentID { get; set; }

        [ForeignKey("StudentID")]
        public Student Student { get; set; }
        [ForeignKey("CourseID")]
        public Course Course { get; set; }
    }
}
