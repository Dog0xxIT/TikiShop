using System.ComponentModel.DataAnnotations;

namespace TikiShop.Model.RequestModels.User;

public class CreateAddressRequest
{
    [Required] public string Receiver { get; set; }

    [Required] public string PhoneNumber { get; set; }

    [Required] public string Street { get; set; }

    [Required] public string City { get; set; }

    [Required] public string State { get; set; }

    [Required] public string Country { get; set; }

    [Required] public string ZipCode { get; set; }
}