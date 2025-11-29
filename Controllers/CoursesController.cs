using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversityPortal.Data;
using UniversityPortal.Models;

namespace UniversityPortal.Controllers;

public class CoursesController : Controller
{
    private readonly ApplicationDbContext _context;

    public CoursesController(ApplicationDbContext context)
    {
        _context = context;
    }

    // GET: Courses
    public async Task<IActionResult> Index(string searchString, string sortOrder)
    {
        ViewData["CurrentFilter"] = searchString;
        ViewData["CodeSortParam"] = string.IsNullOrEmpty(sortOrder) ? "code_desc" : "";
        ViewData["NameSortParam"] = sortOrder == "name" ? "name_desc" : "name";
        ViewData["CreditsSortParam"] = sortOrder == "credits" ? "credits_desc" : "credits";

        var courses = _context.Courses.Where(c => c.IsActive);

        // Search functionality
        if (!string.IsNullOrEmpty(searchString))
        {
            courses = courses.Where(c => 
                c.CourseCode.Contains(searchString) ||
                c.CourseName.Contains(searchString) ||
                c.Instructor.Contains(searchString));
        }

        // Sort functionality
        courses = sortOrder switch
        {
            "code_desc" => courses.OrderByDescending(c => c.CourseCode),
            "name" => courses.OrderBy(c => c.CourseName),
            "name_desc" => courses.OrderByDescending(c => c.CourseName),
            "credits" => courses.OrderBy(c => c.Credits),
            "credits_desc" => courses.OrderByDescending(c => c.Credits),
            _ => courses.OrderBy(c => c.CourseCode)
        };

        return View(await courses.ToListAsync());
    }

    // GET: Courses/Details/5
    public async Task<IActionResult> Details(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var course = await _context.Courses
            .Include(c => c.Enrollments)
            .ThenInclude(e => e.Student)
            .FirstOrDefaultAsync(m => m.Id == id);

        if (course == null)
        {
            return NotFound();
        }

        return View(course);
    }
}
