using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskProjectOrion.Models;
using Task = TaskProjectOrion.Models.Task;

namespace TaskProjectOrion.Data;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
    {
        
    }
    public DbSet<Project> Projects { get; set; }
    public DbSet<Task>  Tasks { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Task>().HasMany(t => t.Users).WithMany(u => u.Tasks); 
    }
}

