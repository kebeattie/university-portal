using System.ComponentModel.DataAnnotations;

namespace UniversityPortal.Models;

public enum EnrolmentStatus
{
    Enrolled,
    Completed,
    Withdrawn,
    Failed
}

public class Enrolment
{
    public int Id { get; set; }

    [Required]
    public int StudentId { get; set; }

    [Required]
    public int CourseId { get; set; }

    [Display(Name = "Enrolment Date")]
    public DateTime EnrolmentDate { get; set; } = DateTime.Now;

    public EnrolmentStatus Status { get; set; } = EnrolmentStatus.Enrolled;

    [Range(0, 100)]
    public decimal? Grade { get; set; }

    // Navigation properties
    public Student Student { get; set; } = null!;
    public Course Course { get; set; } = null!;
}
