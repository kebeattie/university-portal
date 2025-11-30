using System.ComponentModel.DataAnnotations;

namespace UniversityPortal.Models;

public class Course
{
    public int Id { get; set; }

    [Required]
    [StringLength(10)]
    [Display(Name = "Course Code")]
    public string CourseCode { get; set; } = string.Empty;

    [Required]
    [StringLength(200)]
    [Display(Name = "Course Name")]
    public string CourseName { get; set; } = string.Empty;

    [StringLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Range(1, 10)]
    public int Credits { get; set; }

    [StringLength(100)]
    public string Instructor { get; set; } = string.Empty;

    [Display(Name = "Maximum Students")]
    public int MaxStudents { get; set; }

    [Display(Name = "Current Enrolled")]
    public int CurrentEnrolled { get; set; }

    [Display(Name = "Is Active")]
    public bool IsActive { get; set; } = true;

    // Navigation property
    public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();
}
