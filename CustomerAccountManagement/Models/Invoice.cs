using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using CustomerAccountManagement.Enums;

namespace CustomerAccountManagement.Models;

public class Invoice
{
    public int Id { get; set; }

    [Required]
    public int ClientId { get; set; }

    public Client Client { get; set; } = null!;

    [Required(ErrorMessage = "Amount is required")]
    [Column(TypeName = "decimal(18,2)")]
    [Range(0.01, (double)decimal.MaxValue, ErrorMessage = "Amount must be greater than 0")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "Currency is required")]
    public Currency Currency { get; set; }

    [Required]
    [MaxLength(500)]
    public string OriginalFileName { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    public string StoredFileName { get; set; } = string.Empty;

    public InvoiceStatus Status { get; set; } = InvoiceStatus.Pending;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
