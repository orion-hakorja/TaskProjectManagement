using Microsoft.AspNetCore.Identity;

namespace TaskProjectOrion.Models;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
    public ICollection<Task> Tasks { get; set; } = new List<Task>();
}