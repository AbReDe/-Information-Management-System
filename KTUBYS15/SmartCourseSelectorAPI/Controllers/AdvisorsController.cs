﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using Microsoft.EntityFrameworkCore;
using SmartCourseSelectorWeb.Models;
using StudentIMS.Models;

namespace SmartCourseSelectorWeb.Controllers
{
    [Route("api/AdvisorController")]
    [ApiController]
    public class AdvisorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdvisorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("ApproveCourses")]
        public IActionResult ApproveCourses(int id)
        {
            // Danışman tarafından onaylanacak dersler burada işlenir.
            var advisor = _context.Advisors
                .Include(a => a.Students)
                .FirstOrDefault(a => a.AdvisorID == id);

            if (advisor == null)
            {
                return NotFound(new { Message = "Advisor not found." });
            }
            return View(advisor);
        }
        [HttpPost("ApproveStudentsCourses")]
        public IActionResult ApproveStudentsCourses(
            [FromForm] List<int> SelectedCourseIds,
            [FromForm] List<string> UncheckedCourseIds,
            [FromForm] int id
            )
        {
            if (SelectedCourseIds == null || !SelectedCourseIds.Any())
            {
                return BadRequest("No courses selected for approval.");
            }

            // Seçilen kursları onayla (IsApproved = true)
            foreach (var courseId in SelectedCourseIds)
            {
                var courseSelection = _context.StudentCourseSelections
                    .FirstOrDefault(s => s.CourseID == courseId);
                if (courseSelection != null)
                {
                    courseSelection.IsApproved = true;
                }
            }

            // Seçilmemiş kursları onayla (IsApproved = false)
            if (UncheckedCourseIds != null && UncheckedCourseIds.Any())
            {
                foreach (var uncheckedCourse in UncheckedCourseIds)
                {
                    var parts = uncheckedCourse.Split(":");
                    int courseId = int.Parse(parts[0]);
                    bool isApproved = bool.Parse(parts[1]);

                    var courseSelection = _context.StudentCourseSelections
                        .FirstOrDefault(s => s.CourseID == courseId);
                    if (courseSelection != null)
                    {
                        courseSelection.IsApproved = isApproved;
                    }
                }
            }

            // UnapprovedSelections tablosundaki kursları sil
            foreach (var courseId in SelectedCourseIds)
            {
                var unapprovedCourse = _context.UnapprovedSelections
                    .FirstOrDefault(s => s.CourseID == courseId);
                if (unapprovedCourse != null)
                {
                    _context.UnapprovedSelections.Remove(unapprovedCourse); // Bu kaydı sil
                }
                
            }

            // Veritabanı değişikliklerini kaydet
            _context.SaveChanges();


            return RedirectToAction("ApproveCourses", "Advisors",new { id = id });
        }





        [HttpGet("ViewStudentCourses/{studentId}")]
        public IActionResult ViewStudentCourses(int studentId)
        {
            // Öğrenciyi ve seçtiği dersleri çekiyoruz
            var student = _context.Students
                .Include(s => s.StudentCourseSelections)
                    .ThenInclude(sc => sc.Course)
                .FirstOrDefault(s => s.StudentID == studentId);

            if (student == null)
            {
                return NotFound(new { Message = "Student not found." });
            }

            return View(student);
        }

        // GET: api/AdvisorController/getAdvisorList
        [HttpGet("getAdvisorList")]
        public async Task<ActionResult<IEnumerable<Advisor>>> GetAdvisors()
        {
            return await _context.Advisors.ToListAsync();
        }

        // GET: api/AdvisorController/getById?id=5
        [HttpGet("getById")]
        public async Task<ActionResult<Advisor>> GetAdvisor(int id)
        {
            var advisor = await _context.Advisors.FindAsync(id);

            if (advisor == null)
            {
                return NotFound(new { Message = "Advisor not found." });
            }

            return advisor;
        }

        // POST: api/AdvisorController
        [HttpPost("CreateAdvisor")]
        public async Task<ActionResult<Advisor>> CreateAdvisor(Advisor advisor)
        {
            _context.Advisors.Add(advisor);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAdvisor", new { id = advisor.AdvisorID }, advisor);
        }

        // PUT: api/AdvisorController/updateAdvisor?id=5
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAdvisor(int id, Advisor advisor)
        {
            if (id != advisor.AdvisorID)
            {
                return BadRequest(new { Message = "Advisor ID mismatch." });
            }

            _context.Entry(advisor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AdvisorExists(id))
                {
                    return NotFound(new { Message = "Advisor not found." });
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE: api/AdvisorController/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAdvisor(int id)
        {
            var advisor = await _context.Advisors.FindAsync(id);
            if (advisor == null)
            {
                return NotFound(new { Message = "Advisor not found." });
            }

            _context.Advisors.Remove(advisor);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool AdvisorExists(int id)
        {
            return _context.Advisors.Any(e => e.AdvisorID == id);
        }
    }
}




//furkan bilici tarafından yapılmıştır