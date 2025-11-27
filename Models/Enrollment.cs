using System.ComponentModel.DataAnnotations;

namespace UniversityPortal.Models;

public enum EnrollmentStatus
{
    Enrolled,
    Completed,
    Dropped,
    Failed
}

public class Enrollment
{
    public int Id { get; set; }

    [Required]
    public int StudentId { get; set; }

    [Required]
    public int CourseId { get; set; }

    [Display(Name = "Enrollment Date")]
    public DateTime EnrollmentDate { get; set; } = DateTime.Now;

    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Enrolled;

    [Range(0, 100)]
    public decimal? Grade { get; set; }

    // Navigation properties
    public Student Student { get; set; } = null!;
    public Course Course { get; set; } = null!;
}
