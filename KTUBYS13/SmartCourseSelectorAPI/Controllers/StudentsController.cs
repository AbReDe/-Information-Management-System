using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartCourseSelectorWeb.Models;
using StudentIMS.Models;
using System;

namespace SmartCourseSelectorWeb.Controllers
{
    [Route("api/StudentController")]
    [ApiController]
    public class StudentsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("SelectCourses")]
        public async Task<IActionResult> SelectCourses([FromQuery] int StudentId)
        {
            if (StudentId == 0)
            {
                return BadRequest("Invalid student ID.");
            }

            var course = await _context.Courses.ToListAsync();
            var student = await _context.Students
                                       .Include(s => s.StudentCourseSelections)
                                           .ThenInclude(sc => sc.Course)
                                       .Include(s => s.Advisor)
                                       .FirstOrDefaultAsync(s => s.StudentID == StudentId);

            if (student == null)
            {
                return NotFound($"Student with ID {StudentId} not found.");
            }

            var sc = new StudentAndCourse
            {
                Course = course,
                Student = student
            };

            return View(sc);
        }


        [HttpPost("SubmitSelectedCourses")]
        public async Task<IActionResult> SubmitSelectedCourses([FromBody] SubmitCoursesRequest request)
        {
            if (request == null || request.SelectedCourseIds == null || !request.SelectedCourseIds.Any())
            {
                return BadRequest("No courses selected.");
            }

            try
            {
                // Öğrenciye ait mevcut kayıtları kontrol et ve sil
                var existingRecords = _context.UnapprovedSelections
                    .Where(x => x.StudentID == request.StudentId)
                    .ToList();

                if (existingRecords.Any())
                {
                    _context.UnapprovedSelections.RemoveRange(existingRecords);
                }

                // Yeni kayıtları ekle
                int maxId = _context.UnapprovedSelections.Any()
                    ? _context.UnapprovedSelections.Max(x => x.ID)
                    : 0;

                foreach (var courseId in request.SelectedCourseIds)
                {
                    maxId++; // Yeni ID'yi belirle

                    var unapprovedSelection = new UnapprovedSelections
                    {
                        ID = maxId,
                        StudentID = request.StudentId,
                        CourseID = courseId
                    };

                    _context.UnapprovedSelections.Add(unapprovedSelection);
                }

                // Değişiklikleri kaydet
                await _context.SaveChangesAsync();

                return Ok(new { Message = "Courses updated successfully!" });
            }
            catch (Exception ex)
            {
                // Hata durumunda geri bildirim ver
                return StatusCode(500, new { Message = "An error occurred while updating courses.", Details = ex.Message });
            }
        }


        [HttpGet("CourseSelection")]
        public async Task<IActionResult> CourseSelection(int id)
        {
           
                var student = await _context.Students
                                             .Include(s => s.StudentCourseSelections) // Seçilen dersler
                                                 .ThenInclude(sc => sc.Course)       // Ders bilgileri
                                             .Include(s => s.Advisor)                // Danışman bilgisi
                                             .Include(s => s.UnapprovedSelections)   // Onaylanmamış dersler
                                                 .ThenInclude(us => us.Course)      // Ders bilgileri
                                             .FirstOrDefaultAsync(s => s.StudentID == id);

                // Öğrenci bulunamadıysa hata mesajı gönder
                if (student == null)
                {
                    return NotFound("Student not found.");
                }

                // Öğrenci modelini View'a gönder
                return View(student);
            
        }



        // GET: api/Students
        [HttpGet("getStudentList")]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudents()
        {
            return await _context.Students.ToListAsync();
        }

        // GET: api/Students/5
        [HttpGet("getById")]
        public async Task<ActionResult<Student>> GetStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);

            if (student == null)
            {
                return NotFound(new { Message = "Student not found." });
            }

            return student;
        }

        // PUT: api/Students/5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(int id, Student student)
        {
            if (id != student.StudentID)
            {
                return BadRequest(new { Message = "Student ID mismatch." });
            }

            _context.Entry(student).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!StudentExists(id))
                {
                    return NotFound(new { Message = "Student not found." });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Students
        [HttpPost]
        public async Task<ActionResult<Student>> CreateStudent(Student student)
        {
            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetStudent", new { id = student.StudentID }, student);
        }

        // DELETE: api/Students/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null)
            {
                return NotFound(new { Message = "Student not found." });
            }

            _context.Students.Remove(student);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool StudentExists(int id)
        {
            return _context.Students.Any(e => e.StudentID == id);
        }
        // GET: api/StudentController/getByAdvisorId
        [HttpGet("getByAdvisorId")]
        public async Task<ActionResult<IEnumerable<Student>>> GetStudentsByAdvisorId(int advisorId)
        {
            // AdvisorID'ye göre filtrelenmiş öğrencileri al
            var students = await _context.Students
                                          .Where(s => s.AdvisorID == advisorId) // AdvisorID ile filtreleme
                                          .ToListAsync();

            // Eğer öğrenciler bulunamadıysa NotFound döner
            if (!students.Any())
            {
                return NotFound(new { Message = $"No students found for AdvisorID {advisorId}." });
            }

            // Öğrenci listesi başarılı şekilde döner
            return Ok(students);
        }

    }
}
