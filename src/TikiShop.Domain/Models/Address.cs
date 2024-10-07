using Catalog.Api.Data.Entities;

namespace TikiShop.Core.Models
{
    public class Address : BaseEntity
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
