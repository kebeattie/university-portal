using Microsoft.AspNetCore.Identity;
using UniversityPortal.Models;

namespace UniversityPortal.Data;

public static class DbSeeder
{
    public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

        // Create roles if they don't exist
        string[] roles = { "Admin", "Student" };
        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        // Create default admin user
        var adminEmail = "admin@university.com";
        var adminUser = await userManager.FindByEmailAsync(adminEmail);

        if (adminUser == null)
        {
            var admin = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(admin, "Admin@123");

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "Admin");
            }
        }
    }

    public static async Task SeedSampleDataAsync(ApplicationDbContext context)
    {
        // Seed sample courses if none exist
        if (!context.Courses.Any())
        {
            var courses = new List<Course>
            {
                new Course
                {
                    CourseCode = "CS101",
                    CourseName = "Introduction to Computer Science",
                    Description = "Fundamental concepts of computer science and programming",
                    Credits = 3,
                    Instructor = "Dr. Smith",
                    MaxStudents = 30,
                    CurrentEnrolled = 0,
                    IsActive = true
                },
                new Course
                {
                    CourseCode = "CS201",
                    CourseName = "Data Structures and Algorithms",
                    Description = "Advanced programming concepts, data structures, and algorithms",
                    Credits = 4,
                    Instructor = "Dr. Johnson",
                    MaxStudents = 25,
                    CurrentEnrolled = 0,
                    IsActive = true
                },
                new Course
                {
                    CourseCode = "MATH101",
                    CourseName = "Calculus I",
                    Description = "Introduction to differential and integral calculus",
                    Credits = 4,
                    Instructor = "Prof. Williams",
                    MaxStudents = 40,
                    CurrentEnrolled = 0,
                    IsActive = true
                },
                new Course
                {
                    CourseCode = "ENG101",
                    CourseName = "English Composition",
                    Description = "Academic writing and critical thinking",
                    Credits = 3,
                    Instructor = "Prof. Davis",
                    MaxStudents = 20,
                    CurrentEnrolled = 0,
                    IsActive = true
                },
                new Course
                {
                    CourseCode = "PHYS101",
                    CourseName = "Physics I",
                    Description = "Mechanics, waves, and thermodynamics",
                    Credits = 4,
                    Instructor = "Dr. Brown",
                    MaxStudents = 35,
                    CurrentEnrolled = 0,
                    IsActive = true
                }
            };

            context.Courses.AddRange(courses);
            await context.SaveChangesAsync();
        }
    }
}
