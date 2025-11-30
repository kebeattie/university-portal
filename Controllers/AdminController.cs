using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityPortal.Data;
using UniversityPortal.Models;

namespace UniversityPortal.Controllers;

[Authorize(Roles = UserRoles.Admin)]
public class AdminController : Controller
{
    private readonly ApplicationDbContext _context;

    public AdminController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Admin
    public async Task<IActionResult> Index()
    {
        var stats = new
        {
            TotalCourses = await _context.Courses.CountAsync(),
            ActiveCourses = await _context.Courses.CountAsync(c => c.IsActive),
            TotalStudents = await _context.Students.CountAsync(),
            TotalEnrolments = await _context.Enrolments.CountAsync(e => e.Status == EnrolmentStatus.Enrolled)
        };

        ViewBag.Stats = stats;

        var recentEnrolments = await _context.Enrolments
            .Include(e => e.Student)
            .Include(e => e.Course)
            .OrderByDescending(e => e.EnrolmentDate)
            .Take(10)
            .ToListAsync();

        return View(recentEnrolments);
    }

    // GET: Admin/CreateCourse
    public IActionResult CreateCourse()
    {
        return View();
    }

    // POST: Admin/CreateCourse
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateCourse([Bind("CourseCode,CourseName,Description,Credits,Instructor,MaxStudents,IsActive")] Course course)
    {
        if (ModelState.IsValid)
        {
            course.CurrentEnrolled = 0;
            _context.Add(course);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Course {course.CourseCode} created successfully!";
            return RedirectToAction(nameof(Index));
        }
        return View(course);
    }

    // GET: Admin/EditCourse/5
    public async Task<IActionResult> EditCourse(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var course = await _context.Courses.FindAsync(id);
        if (course == null)
        {
            return NotFound();
        }
        return View(course);
    }

    // POST: Admin/EditCourse/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditCourse(int id, [Bind("Id,CourseCode,CourseName,Description,Credits,Instructor,MaxStudents,CurrentEnrolled,IsActive")] Course course)
    {
        if (id != course.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                _context.Update(course);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = $"Course {course.CourseCode} updated successfully!";
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CourseExists(course.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));
        }
        return View(course);
    }

    // GET: Admin/DeleteCourse/5
    public async Task<IActionResult> DeleteCourse(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var course = await _context.Courses
            .Include(c => c.Enrolments)
            .FirstOrDefaultAsync(m => m.Id == id);
        if (course == null)
        {
            return NotFound();
        }

        return View(course);
    }

    // POST: Admin/DeleteCourse/5
    [HttpPost, ActionName("DeleteCourse")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCourseConfirmed(int id)
    {
        var course = await _context.Courses.FindAsync(id);
        if (course != null)
        {
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = $"Course {course.CourseCode} deleted successfully!";
        }

        return RedirectToAction(nameof(Index));
    }

    private bool CourseExists(int id)
    {
        return _context.Courses.Any(e => e.Id == id);
    }
}
