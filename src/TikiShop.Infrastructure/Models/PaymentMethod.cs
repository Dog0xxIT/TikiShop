namespace TikiShop.Infrastructure.Models;

public class PaymentMethod : BaseEntity
{
    public string Alias { get; set; }
    public string CardNumber { get; set; }
    public string SecurityNumber { get; set; }
    public string CardHolderName { get; set; }
    public DateTime Expiration { get; set; }
    public int CardTypeId { get; set; }
    public CardType CardType { get; set; }
}