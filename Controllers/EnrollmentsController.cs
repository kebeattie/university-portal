using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityPortal.Data;
using UniversityPortal.Models;

namespace UniversityPortal.Controllers;

[Authorize(Roles = UserRoles.Student)]
public class EnrollmentsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;

    public EnrollmentsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    // GET: Enrollments/MyEnrollments
    public async Task<IActionResult> MyEnrollments()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        var student = await _context.Students
            .Include(s => s.Enrollments)
            .ThenInclude(e => e.Course)
            .FirstOrDefaultAsync(s => s.UserId == user.Id);

        if (student == null)
        {
            // Redirect to profile creation if student profile doesn't exist
            return RedirectToAction(nameof(CreateProfile));
        }

        return View(student);
    }

    // GET: Enrollments/CreateProfile
    [AllowAnonymous]
    public IActionResult CreateProfile()
    {
        return View();
    }

    // POST: Enrollments/CreateProfile
    [HttpPost]
    [ValidateAntiForgeryToken]
    [AllowAnonymous]
    public async Task<IActionResult> CreateProfile([Bind("FirstName,LastName,StudentNumber,DateOfBirth,Major")] Student student)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        // Check if student profile already exists
        var existingStudent = await _context.Students.FirstOrDefaultAsync(s => s.UserId == user.Id);
        if (existingStudent != null)
        {
            return RedirectToAction(nameof(MyEnrollments));
        }

        if (ModelState.IsValid)
        {
            student.UserId = user.Id;
            student.EnrollmentDate = DateTime.Now;

            _context.Add(student);
            await _context.SaveChangesAsync();

            // Add user to Student role if not already
            if (!await _userManager.IsInRoleAsync(user, UserRoles.Student))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.Student);
            }

            TempData["SuccessMessage"] = "Student profile created successfully! You can now enroll in courses.";
            return RedirectToAction("Index", "Courses");
        }
        return View(student);
    }

    // GET: Enrollments/Enroll?courseId=1
    public async Task<IActionResult> Enroll(int? courseId)
    {
        if (courseId == null)
        {
            return NotFound();
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == user.Id);
        if (student == null)
        {
            TempData["ErrorMessage"] = "Please create your student profile first.";
            return RedirectToAction(nameof(CreateProfile));
        }

        var course = await _context.Courses.FindAsync(courseId);
        if (course == null)
        {
            return NotFound();
        }

        // Check if already enrolled
        var existingEnrollment = await _context.Enrollments
            .FirstOrDefaultAsync(e => e.StudentId == student.Id && e.CourseId == courseId);

        if (existingEnrollment != null)
        {
            TempData["ErrorMessage"] = "You are already enrolled in this course.";
            return RedirectToAction("Details", "Courses", new { id = courseId });
        }

        // Check if course is full
        if (course.CurrentEnrolled >= course.MaxStudents)
        {
            TempData["ErrorMessage"] = "This course is full.";
            return RedirectToAction("Details", "Courses", new { id = courseId });
        }

        ViewBag.Course = course;
        ViewBag.Student = student;
        return View();
    }

    // POST: Enrollments/Enroll
    [HttpPost, ActionName("Enroll")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EnrollConfirmed(int courseId)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == user.Id);
        if (student == null)
        {
            return NotFound();
        }

        var course = await _context.Courses.FindAsync(courseId);
        if (course == null)
        {
            return NotFound();
        }

        // Double-check enrollment constraints
        var existingEnrollment = await _context.Enrollments
            .FirstOrDefaultAsync(e => e.StudentId == student.Id && e.CourseId == courseId);

        if (existingEnrollment != null)
        {
            TempData["ErrorMessage"] = "You are already enrolled in this course.";
            return RedirectToAction(nameof(MyEnrollments));
        }

        if (course.CurrentEnrolled >= course.MaxStudents)
        {
            TempData["ErrorMessage"] = "This course is full.";
            return RedirectToAction(nameof(MyEnrollments));
        }

        // Create enrollment
        var enrollment = new Enrollment
        {
            StudentId = student.Id,
            CourseId = courseId,
            EnrollmentDate = DateTime.Now,
            Status = EnrollmentStatus.Enrolled
        };

        _context.Enrollments.Add(enrollment);

        // Update course enrollment count
        course.CurrentEnrolled++;
        _context.Update(course);

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Successfully enrolled in {course.CourseCode} - {course.CourseName}!";
        return RedirectToAction(nameof(MyEnrollments));
    }

    // POST: Enrollments/Drop/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Drop(int id)
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == user.Id);
        if (student == null)
        {
            return NotFound();
        }

        var enrollment = await _context.Enrollments
            .Include(e => e.Course)
            .FirstOrDefaultAsync(e => e.Id == id && e.StudentId == student.Id);

        if (enrollment == null)
        {
            return NotFound();
        }

        // Update enrollment status
        enrollment.Status = EnrollmentStatus.Dropped;
        _context.Update(enrollment);

        // Decrease course enrollment count
        enrollment.Course.CurrentEnrolled--;
        _context.Update(enrollment.Course);

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Successfully dropped {enrollment.Course.CourseCode}.";
        return RedirectToAction(nameof(MyEnrollments));
    }
}
