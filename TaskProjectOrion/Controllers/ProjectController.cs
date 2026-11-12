using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskProjectOrion.Data;
using TaskProjectOrion.Models;

namespace TaskProjectOrion.Controllers;

[Authorize]
public class ProjectController : Controller
{
    private readonly AppDbContext context;

    public ProjectController(AppDbContext db)
    {
        this.context = db;
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Index()
    {
        var projects = await context.Projects.Where(p=>p.userId == User.FindFirstValue(ClaimTypes.NameIdentifier)).ToListAsync();
        return View(projects);
    }

    [Authorize(Roles = "Admin")]
    public IActionResult Create()
    {
        return View();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Project model)
    {
        var id = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (id == null)
        {
            TempData["Error"] = "You need to login to access this project!";
            return RedirectToAction("Login", "Account");
        }
        model.userId = id;
        
        var project = new Project
        {
            Name = model.Name,
            Description = model.Description,
            StartDate = DateTime.Now,
            deadline = model.deadline,
            userId = model.userId,
            status = "Initialized"
        };
        await context.Projects.AddAsync(project);
        await  context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Edit(int? id)
    {
        if (id == null)
        {
            return RedirectToAction(nameof(Index));
        }
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var project = await context.Projects.FirstOrDefaultAsync(p=> p.Id == id && p.userId == userId);
        if (project == null)
        {
            TempData["Error"] = "Could not find project!";
            return RedirectToAction("Index");
        }
        return View(project);
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(Project model,int? id)
    {
        var project = await context.Projects.FirstOrDefaultAsync(p => p.Id == id && p.userId == User.FindFirstValue(ClaimTypes.NameIdentifier));
        if (project == null)
        {
            TempData["Error"] = "Could not find project!";
            return RedirectToAction("Index");
        }
        project.deadline = model.deadline;
        project.Description = model.Description;
        project.Name = model.Name;
        project.status = model.status;
        await context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var project = await context.Projects.FirstOrDefaultAsync(p=>p.Id == id);
        if (project == null)
        {
            TempData["Error"] = "Could not find project!";
            return RedirectToAction("Index");
        }
        context.Projects.Remove(project);
        await context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Search(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return RedirectToAction(nameof(Index));

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var search = $"%{query.Trim()}%";

        var projects = await context.Projects
            .Where(p => p.userId == userId &&
                        (EF.Functions.Like(p.Name, search) ||
                         EF.Functions.Like(p.Description, search) ||
                         EF.Functions.Like(p.status, search)))
            .ToListAsync();

        return View("Index", projects);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int? id)
    {
        var project = await context.Projects.Include(p=>p.Tasks).ThenInclude(t=>t.Users).FirstOrDefaultAsync(p=>p.Id == id);
        if (project == null)
        {
            TempData["Error"] = "Couldn't find project!";
            return RedirectToAction("Index");
        }
        var tasks = project.Tasks;
        ViewBag.ProjectName = project.Name;
        return View(tasks);
    }
}