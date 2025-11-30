using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace UniversityPortal.Models;

public class Student
{
    public int Id { get; set; }

    public string UserId { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    [Display(Name = "First Name")]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [StringLength(50)]
    [Display(Name = "Last Name")]
    public string LastName { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    [Display(Name = "Student Number")]
    public string StudentNumber { get; set; } = string.Empty;

    [DataType(DataType.Date)]
    [Display(Name = "Date of Birth")]
    public DateTime DateOfBirth { get; set; }

    [StringLength(100)]
    public string Programme { get; set; } = string.Empty;

    [Display(Name = "Enrolment Date")]
    public DateTime EnrolmentDate { get; set; } = DateTime.Now;

    // Navigation properties
    [ValidateNever]
    public IdentityUser User { get; set; } = null!;
    
    [ValidateNever]
    public ICollection<Enrolment> Enrolments { get; set; } = new List<Enrolment>();
}
