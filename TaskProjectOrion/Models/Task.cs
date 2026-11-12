using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskProjectOrion.Models;

public class Task
{
    [Required] [Key] public int Id { get; set; }
    [Required] public string Title { get; set; }
    [Required] public string Description { get; set; }
    public ICollection<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
    [Required] public int ProjectId { get; set; }
    [ForeignKey("ProjectId")] public Project Project { get; set; }
    [Required] public string status { get; set; }
    [Required] public string priority { get; set; }
}