using TikiShop.Infrastructure.Common;

namespace TikiShop.Infrastructure.Models
{
    public class Address : BaseEntity
    {
        public string Receiver { get; set; }
        public string PhoneNumber { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
