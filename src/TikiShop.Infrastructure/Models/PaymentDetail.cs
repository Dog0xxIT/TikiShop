namespace TikiShop.Infrastructure.Models;

public class PaymentDetail : BaseEntity
{
    public double Amount { get; set; }
    public int PaymentProviderId { get; set; }
    public double TransactionId { get; set; }
    public string PaymentCode { get; set; }
    public DateTime TransactionDate { get; set; }
    public int Status { get; set; }
    public PaymentProvider PaymentProvider { get; set; }
}