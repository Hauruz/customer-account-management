using CustomerAccountManagement.Data;
using CustomerAccountManagement.Enums;
using CustomerAccountManagement.Models;
using CustomerAccountManagement.ViewModels.Invoices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CustomerAccountManagement.Controllers;

public class InvoicesController : Controller
{
    private readonly ApplicationDbContext _db;
    private readonly IWebHostEnvironment _env;
    private readonly IConfiguration _config;

    public InvoicesController(
        ApplicationDbContext db,
        IWebHostEnvironment env,
        IConfiguration config)
    {
        _db     = db;
        _env    = env;
        _config = config;
    }

    // GET: /Invoices?clientId=1&currency=USD
    public async Task<IActionResult> Index(int? clientId, string? currency)
    {
        var query = _db.Invoices
            .Include(i => i.Client)
            .AsQueryable();

        if (clientId.HasValue)
            query = query.Where(i => i.ClientId == clientId.Value);

        if (!string.IsNullOrWhiteSpace(currency) &&
            Enum.TryParse<Currency>(currency, out var parsedCurrency))
            query = query.Where(i => i.Currency == parsedCurrency);

        var invoices = await query
            .OrderByDescending(i => i.CreatedAt)
            .Select(i => new InvoiceRowViewModel
            {
                Id               = i.Id,
                ClientName       = i.Client.Name,
                ClientEmail      = i.Client.Email,
                Amount           = i.Amount,
                Currency         = i.Currency.ToString(),
                OriginalFileName = i.OriginalFileName,
                StoredFileName   = i.StoredFileName,
                Status           = i.Status.ToString(),
                CreatedAt        = i.CreatedAt
            })
            .ToListAsync();

        var clients = await _db.Clients
            .OrderBy(c => c.Name)
            .Select(c => new SelectListItem
            {
                Value    = c.Id.ToString(),
                Text     = c.Name,
                Selected = c.Id == clientId
            })
            .ToListAsync();

        clients.Insert(0, new SelectListItem { Value = "", Text = "— All Clients —" });

        var currencyOptions = Enum.GetNames<Currency>()
            .Select(c => new SelectListItem
            {
                Value    = c,
                Text     = c,
                Selected = c == currency
            })
            .ToList();

        currencyOptions.Insert(0, new SelectListItem { Value = "", Text = "— All Currencies —" });

        var vm = new InvoiceIndexViewModel
        {
            Invoices         = invoices,
            SelectedClientId = clientId,
            SelectedCurrency = currency,
            ClientOptions    = clients,
            CurrencyOptions  = currencyOptions
        };

        return View(vm);
    }

    // GET: /Invoices/Create
    public async Task<IActionResult> Create()
    {
        var vm = await BuildCreateViewModelAsync();
        return View(vm);
    }

    // POST: /Invoices/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(InvoiceCreateViewModel model)
    {
        ValidatePdfFile(model);

        if (!ModelState.IsValid)
        {
            var vm = await BuildCreateViewModelAsync();
            vm.ClientId = model.ClientId;
            vm.Amount   = model.Amount;
            vm.Currency = model.Currency;
            return View(vm);
        }

        var (storedName, originalName) = await SavePdfAsync(model.PdfFile!);

        var invoice = new Invoice
        {
            ClientId         = model.ClientId!.Value,
            Amount           = model.Amount!.Value,
            Currency         = model.Currency!.Value,
            OriginalFileName = originalName,
            StoredFileName   = storedName
        };

        _db.Invoices.Add(invoice);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Invoice created successfully.";
        return RedirectToAction(nameof(Index));
    }

    // POST: /Invoices/Delete/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var invoice = await _db.Invoices.FindAsync(id);
        if (invoice is null)
            return NotFound();

        DeletePdfFile(invoice.StoredFileName);

        _db.Invoices.Remove(invoice);
        await _db.SaveChangesAsync();

        TempData["Success"] = "Invoice deleted.";
        return RedirectToAction(nameof(Index));
    }

    // POST: /Invoices/UpdateStatus/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, InvoiceStatus status)
    {
        var invoice = await _db.Invoices.FindAsync(id);
        if (invoice is null)
            return NotFound();

        invoice.Status = status;
        await _db.SaveChangesAsync();

        TempData["Success"] = $"Invoice #{id} marked as {status}.";
        return RedirectToAction(nameof(Index));
    }

    // ──────────────── Private helpers ────────────────

    private async Task<InvoiceCreateViewModel> BuildCreateViewModelAsync()
    {
        var clients = await _db.Clients
            .OrderBy(c => c.Name)
            .Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text  = c.Name
            })
            .ToListAsync();

        clients.Insert(0, new SelectListItem { Value = "", Text = "— Select Client —" });

        var currencies = Enum.GetNames<Currency>()
            .Select(c => new SelectListItem { Value = c, Text = c })
            .ToList();

        currencies.Insert(0, new SelectListItem { Value = "", Text = "— Select Currency —" });

        return new InvoiceCreateViewModel
        {
            Clients    = clients,
            Currencies = currencies
        };
    }

    private void ValidatePdfFile(InvoiceCreateViewModel model)
    {
        if (model.PdfFile is null || model.PdfFile.Length == 0)
        {
            ModelState.AddModelError(nameof(model.PdfFile), "PDF file is required.");
            return;
        }

        var extension = Path.GetExtension(model.PdfFile.FileName).ToLowerInvariant();
        if (extension != ".pdf")
        {
            ModelState.AddModelError(nameof(model.PdfFile), "Only PDF files are allowed.");
            return;
        }

        if (model.PdfFile.ContentType is not ("application/pdf" or "application/x-pdf"))
        {
            ModelState.AddModelError(nameof(model.PdfFile),
                "Invalid file type. Please upload a valid PDF.");
        }
    }

    private async Task<(string storedName, string originalName)> SavePdfAsync(IFormFile file)
    {
        var uploadFolder = Path.Combine(
            _env.WebRootPath,
            _config["FileStorage:UploadPath"] ?? "uploads");

        Directory.CreateDirectory(uploadFolder);

        var originalName = Path.GetFileName(file.FileName);
        var storedName   = $"{Guid.NewGuid()}.pdf";
        var fullPath     = Path.Combine(uploadFolder, storedName);

        await using var stream = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(stream);

        return (storedName, originalName);
    }

    private void DeletePdfFile(string storedFileName)
    {
        var uploadFolder = Path.Combine(
            _env.WebRootPath,
            _config["FileStorage:UploadPath"] ?? "uploads");

        var fullPath = Path.Combine(uploadFolder, storedFileName);

        if (System.IO.File.Exists(fullPath))
            System.IO.File.Delete(fullPath);
    }
}
