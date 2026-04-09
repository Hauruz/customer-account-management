using System.ComponentModel.DataAnnotations;
using CustomerAccountManagement.Enums;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CustomerAccountManagement.ViewModels.Invoices;

public class InvoiceCreateViewModel
{
    [Required(ErrorMessage = "Please select a client")]
    [Display(Name = "Client")]
    public int? ClientId { get; set; }

    [Required(ErrorMessage = "Amount is required")]
    [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    [Display(Name = "Amount")]
    public decimal? Amount { get; set; }

    [Required(ErrorMessage = "Please select a currency")]
    [Display(Name = "Currency")]
    public Currency? Currency { get; set; }

    [Display(Name = "PDF File")]
    public IFormFile? PdfFile { get; set; }

    public List<SelectListItem> Clients { get; set; } = new();
    public List<SelectListItem> Currencies { get; set; } = new();
}
