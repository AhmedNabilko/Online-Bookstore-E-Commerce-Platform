// 1. Create Models/ViewModels/AddressVM.cs
using E_commerceEntity.Entity;

namespace E_commerce.Models.ViewModels
{
    public class AddressVM
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }

        public AddressVM() { }

        // Smart Constructor: Maps Entity -> VM
        public AddressVM(Address address)
        {
            if (address != null)
            {
                Street = address.Street;
                City = address.City;
                Zip = address.Zip;
                Country = address.Country;
            }
        }

        // Helper: Maps VM -> Entity
        public Address ToEntity(string userId)
        {
            return new Address { UserId = userId, Street = Street, City = City, Zip = Zip, Country = Country };
        }
    }
}