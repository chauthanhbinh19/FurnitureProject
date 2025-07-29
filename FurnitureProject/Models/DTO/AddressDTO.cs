using FurnitureProject.Helper;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace FurnitureProject.Models.DTO
{
    public class AddressDTO
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        [Display(Name = AppConstants.Display.AddressStreet)]
        public string? Street { get; set; }
        [Display(Name = AppConstants.Display.AddressWard)]
        public string? Ward { get; set; }
        [Display(Name = AppConstants.Display.AddressDistrict)]
        public string? District { get; set; }
        [Display(Name = AppConstants.Display.AddressCity)]
        public string? City { get; set; }
        [Display(Name = AppConstants.Display.AddressCountry)]
        public string? Country { get; set; }
        [Display(Name = AppConstants.Display.AddressPostalCode)]
        public string? PostalCode { get; set; }
        public bool? IsDefault { get; set; } = false;
    }
}
