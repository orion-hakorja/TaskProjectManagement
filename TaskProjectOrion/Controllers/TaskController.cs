using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TaskProjectOrion.Data;
using TaskProjectOrion.Models;
using Task = TaskProjectOrion.Models.Task;

namespace TaskProjectOrion.Controllers;

[Authorize]
public class TaskController : Controller
{
    private readonly AppDbContext context;
    private readonly UserManager<ApplicationUser> userManager;
    private async System.Threading.Tasks.Task LoadDropdowns()
    {
        ViewBag.Users = await userManager.Users
            .Select(u => new SelectListItem
            {
                Value = u.Id,
                Text = u.UserName
            })
            .ToListAsync();

        ViewBag.Projects = await context.Projects
            .Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = p.Name
            })
            .ToListAsync();
    }
    public TaskController(AppDbContext Ccontext, UserManager<ApplicationUser> userManagerr)
    {
        this.userManager = userManagerr;
        this.context = Ccontext;
    }
    [Authorize(Roles = "User")]
    public async Task<IActionResult> Index()
    {    
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var tasks = await context.Tasks.Include(t=> t.Project)
            .Where(t => t.Users.Any(u => u.Id == userId))
            .ToListAsync();

        return View(tasks);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AdminIndex()
    {
        var tasks = await context.Tasks.Include(t=>t.Users).Include(t=>t.Project).ToListAsync();
        return View(tasks);
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create()
    {
        await LoadDropdowns();
        return View();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Task task,List<string> SelectedUserIds)
    {

        var selectedUsers = await userManager.Users.Where(u => SelectedUserIds.Contains(u.Id)).ToListAsync();
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var tasking = new Task
        {
        Title = task.Title,
        Description =  task.Description,
        status = task.status,
        priority =  task.priority,
        ProjectId =  task.ProjectId,
        Users = selectedUsers,
        };
        context.Tasks.Add(tasking);
        await context.SaveChangesAsync();
        return RedirectToAction("AdminIndex");
    }
}
