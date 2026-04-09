using Microsoft.AspNetCore.Mvc.Rendering;

namespace CustomerAccountManagement.ViewModels.Invoices;

public class InvoiceIndexViewModel
{
    public List<InvoiceRowViewModel> Invoices { get; set; } = new();

    public int? SelectedClientId { get; set; }
    public string? SelectedCurrency { get; set; }

    public List<SelectListItem> ClientOptions { get; set; } = new();
    public List<SelectListItem> CurrencyOptions { get; set; } = new();
}

public class InvoiceRowViewModel
{
    public int Id { get; set; }
    public string ClientName { get; set; } = string.Empty;
    public string ClientEmail { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public string OriginalFileName { get; set; } = string.Empty;
    public string StoredFileName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
