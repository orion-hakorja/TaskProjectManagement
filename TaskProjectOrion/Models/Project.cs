using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskProjectOrion.Models;

public class Project
{
    [Required]
    [Key]
    public int Id { get; set; }
    [Required]
    public string Name { get; set; }
    [Required]
    public string Description { get; set; }
    [Required]
    public DateTime StartDate { get; set; }
    [Required]
    public string status { get; set; }
    public DateTime? deadline { get; set; }
    [Required]
    public string userId{get;set;} 
    [ForeignKey(nameof(userId))]
    public ApplicationUser user { get; set; }
    public List<Task> Tasks { get; set; }
}