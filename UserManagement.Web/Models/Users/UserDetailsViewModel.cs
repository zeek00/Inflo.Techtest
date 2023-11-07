using System;

namespace UserManagement.Web.Models.Users;

public class UserDetailsViewModel
{
    public long Id { get; set; }
    public string Forename { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime DateOfBirth { get; set; }
}
