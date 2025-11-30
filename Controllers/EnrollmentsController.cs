using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityPortal.Data;
using UniversityPortal.Models;

namespace UniversityPortal.Controllers;

[Authorize]
public class EnrollmentsController : Controller
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;

    public EnrollmentsController(ApplicationDbContext context, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
    {
        _context = context;
        _userManager = userManager;
        _signInManager = signInManager;
    }

    // GET: Enrollments/MyEnrollments
    [Authorize(Roles = UserRoles.Student)]
    public async Task<IActionResult> MyEnrollments()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return NotFound();
        }

        var student = await _context.Students
            .Include(s => s.Enrolments)
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
    public IActionResult CreateProfile()
    {
        return View();
    }

    // POST: Enrollments/CreateProfile
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateProfile([Bind("FirstName,LastName,StudentNumber,DateOfBirth,Programme")] Student student)
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
            student.EnrolmentDate = DateTime.Now;

            _context.Add(student);
            await _context.SaveChangesAsync();

            // Add user to Student role if not already
            if (!await _userManager.IsInRoleAsync(user, UserRoles.Student))
            {
                await _userManager.AddToRoleAsync(user, UserRoles.Student);
                
                // Refresh the sign-in to update the role claims
                await _signInManager.RefreshSignInAsync(user);
            }

            TempData["SuccessMessage"] = "Student profile created successfully! You can now enrol in courses.";
            return RedirectToAction("Index", "Home");
        }
        return View(student);
    }

    // GET: Enrollments/Enroll?courseId=1
    [Authorize(Roles = UserRoles.Student)]
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
        var existingEnrolment = await _context.Enrolments
            .FirstOrDefaultAsync(e => e.StudentId == student.Id && e.CourseId == courseId);

        if (existingEnrolment != null)
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
    [Authorize(Roles = UserRoles.Student)]
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

        // Double-check enrolment constraints
        var existingEnrolment = await _context.Enrolments
            .FirstOrDefaultAsync(e => e.StudentId == student.Id && e.CourseId == courseId);

        if (existingEnrolment != null)
        {
            TempData["ErrorMessage"] = "You are already enrolled in this course.";
            return RedirectToAction(nameof(MyEnrollments));
        }

        if (course.CurrentEnrolled >= course.MaxStudents)
        {
            TempData["ErrorMessage"] = "This course is full.";
            return RedirectToAction(nameof(MyEnrollments));
        }

        // Create enrolment
        var enrolment = new Enrolment
        {
            StudentId = student.Id,
            CourseId = courseId,
            EnrolmentDate = DateTime.Now,
            Status = EnrolmentStatus.Enrolled
        };

        _context.Enrolments.Add(enrolment);

        // Update course enrolment count
        course.CurrentEnrolled++;
        _context.Update(course);

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Successfully enrolled in {course.CourseCode} - {course.CourseName}!";
        return RedirectToAction(nameof(MyEnrollments));
    }

    // POST: Enrollments/Drop/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = UserRoles.Student)]
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

        var enrolment = await _context.Enrolments
            .Include(e => e.Course)
            .FirstOrDefaultAsync(e => e.Id == id && e.StudentId == student.Id);

        if (enrolment == null)
        {
            return NotFound();
        }

        // Update enrolment status
        enrolment.Status = EnrolmentStatus.Withdrawn;
        _context.Update(enrolment);

        // Decrease course enrolment count
        enrolment.Course.CurrentEnrolled--;
        _context.Update(enrolment.Course);

        await _context.SaveChangesAsync();

        TempData["SuccessMessage"] = $"Successfully withdrawn from {enrolment.Course.CourseCode}.";
        return RedirectToAction(nameof(MyEnrollments));
    }
}
