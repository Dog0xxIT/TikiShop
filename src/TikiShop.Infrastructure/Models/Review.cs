namespace TikiShop.Infrastructure.Models;

public class Review : BaseEntity
{
    public string RoomId { get; set; }
    public Order Order { get; set; }
}