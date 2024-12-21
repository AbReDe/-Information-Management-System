namespace StudentIMS.Models
{
    public class SubmitCoursesRequest
    {
        public int StudentId { get; set; }
        public List<int> SelectedCourseIds { get; set; }
    }

}
