using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace TikiShop.Infrastructure.Models;

public class Order : BaseEntity
{
    [ForeignKey(nameof(Buyer))] public int BuyerId { get; set; }

    public int PaymentDetailId { get; set; }
    public int AddressId { get; set; }
    public DateTime OrderDate { get; set; }
    public OrderStatus OrderStatus { get; set; }
    public string Description { get; set; }
    public double Total { get; set; }

    [DeleteBehavior(DeleteBehavior.NoAction)]
    public User Buyer { get; set; }

    public Address Address { get; set; }
    public PaymentDetail PaymentDetail { get; set; }
    public List<OrderItem> OrderItems { get; set; }
}