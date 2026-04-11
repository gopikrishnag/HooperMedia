using HooperMedia.Core.Entities;

namespace HooperMedia.Infrastructure.Services.Interfaces
{
    public interface IAddressService
    {
        Task<Address?> GetAddressByIdAsync(int addressId);
        Task<IEnumerable<Address>> GetAllAddressesAsync();
        Task<IEnumerable<Address>> GetAddressesByPersonIdAsync(int personId);
        Task<Address> CreateAddressAsync(Address address);
        Task<Address> UpdateAddressAsync(Address address);
        Task<bool> DeleteAddressAsync(int addressId);
    }
}
