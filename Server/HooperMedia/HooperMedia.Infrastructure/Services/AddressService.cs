using HooperMedia.Core.BusinessRules;
using HooperMedia.Core.Constants;
using HooperMedia.Core.Entities;
using HooperMedia.Core.Extensions;
using HooperMedia.Infrastructure.Repositories.Interfaces;
using HooperMedia.Infrastructure.Services.Interfaces;

namespace HooperMedia.Infrastructure.Services
{
    public class AddressService(IAddressRepository addressRepository) : IAddressService
    {
        public async Task<Address> CreateAddressAsync(Address address)
        {
            ValidateAddress(address);
            return await addressRepository.AddAsync(address);
        }

        public async Task<bool> DeleteAddressAsync(int addressId)
        {
            return await addressRepository.DeleteAsync(addressId);
        }

        public async Task<Address?> GetAddressByIdAsync(int addressId)
        {
            return await addressRepository.GetByIdAsync(addressId);
        }

        public async Task<IEnumerable<Address>> GetAddressesByPersonIdAsync(int personId)
        {
            return await addressRepository.GetByPersonIdAsync(personId);
        }

        public async Task<IEnumerable<Address>> GetAllAddressesAsync()
        {
            return await addressRepository.GetAllAsync();
        }

        public async Task<Address> UpdateAddressAsync(Address address)
        {
            ValidateAddress(address);
            return await addressRepository.UpdateAsync(address);
        }

        //TODO:  keep it separate class for validations
        private static void ValidateAddress(Address address)
        {
            if (string.IsNullOrWhiteSpace(address.AddressLine1) || address.AddressLine1.Length > AddressBusinessRules.AddressLineMaxLength)
            {
                throw new ArgumentException(ValidationErrorMessages.AddressLine1Invalid);
            }

            if (!address.AddressLine2.IsValidOptionalString(AddressBusinessRules.AddressLineMaxLength))
            {
                throw new ArgumentException(ValidationErrorMessages.AddressLine2Invalid);
            }

            if (string.IsNullOrWhiteSpace(address.TownOrCity) || address.TownOrCity.Length > AddressBusinessRules.AddressLineMaxLength)
            {
                throw new ArgumentException(ValidationErrorMessages.TownOrCityInvalid);
            }

            if (string.IsNullOrWhiteSpace(address.ZipOrPostCode) || address.ZipOrPostCode.Length > AddressBusinessRules.ZipOrPostCodeMaxLength)
            {
                throw new ArgumentException(ValidationErrorMessages.ZipOrPostCodeInvalid);
            }

            if (string.IsNullOrWhiteSpace(address.Country) || address.Country.Length > AddressBusinessRules.CountryMaxLength)
            {
                throw new ArgumentException(ValidationErrorMessages.CountryInvalid);
            }
        }
    }
}
