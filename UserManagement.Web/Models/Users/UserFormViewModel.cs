using System.ComponentModel.DataAnnotations;
//using System.Xml.Linq;
using System;

namespace UserManagement.Web.Models.Users;




public class UserFormViewModel
{
    [Required(ErrorMessage = "The Forename field is required.")]
    public string Forename { get; set; } = string.Empty;

    [Required(ErrorMessage = "The Surname field is required.")]
    public string Surname { get; set; } = string.Empty;

    [Required(ErrorMessage = "The Email field is required.")]
    [EmailAddress(ErrorMessage = "Invalid email address.")]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Is Active")]
    public bool IsActive { get; set; }

    [Display(Name = "Date of Birth")]
    [DataType(DataType.Date)]
    public DateTime DateOfBirth { get; set; }
}
