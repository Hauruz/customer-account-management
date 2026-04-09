using CustomerAccountManagement.Data;
using CustomerAccountManagement.Models;
using CustomerAccountManagement.ViewModels.Clients;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CustomerAccountManagement.Controllers;

public class ClientsController : Controller
{
    private readonly ApplicationDbContext _db;

    public ClientsController(ApplicationDbContext db)
    {
        _db = db;
    }

    // GET: /Clients
    public async Task<IActionResult> Index()
    {
        var clients = await _db.Clients
            .OrderBy(c => c.Name)
            .ToListAsync();

        return View(clients);
    }

    // GET: /Clients/Create
    public IActionResult Create() => View(new ClientCreateViewModel());

    // POST: /Clients/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ClientCreateViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var normalizedEmail = model.Email.Trim().ToLowerInvariant();

        var emailExists = await _db.Clients
            .AnyAsync(c => c.Email == normalizedEmail);

        if (emailExists)
        {
            ModelState.AddModelError(nameof(model.Email),
                "A client with this email already exists.");
            return View(model);
        }

        var client = new Client
        {
            Name  = model.Name.Trim(),
            Email = normalizedEmail
        };

        _db.Clients.Add(client);
        await _db.SaveChangesAsync();

        TempData["Success"] = $"Client \"{client.Name}\" created successfully.";
        return RedirectToAction(nameof(Index));
    }
}
