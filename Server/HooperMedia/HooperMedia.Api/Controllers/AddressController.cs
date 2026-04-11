using HooperMedia.Api.DTOs;
using HooperMedia.Core.Entities;
using HooperMedia.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HooperMedia.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AddressController(ILogger<AddressController> logger,
        IAddressService addressService,
        IPersonService personService) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AddressDto>>> GetAllAddresses()
        {
            logger.LogInformation("Getting all addresses");
            var addresses = await addressService.GetAllAddressesAsync();
            var addressDtos = addresses.Select(MapToAddressDto).ToList();
            return Ok(addressDtos);
        }

        [HttpGet("{addressId}")]
        public async Task<ActionResult<AddressDto>> GetAddress(int addressId)
        {
            logger.LogInformation("Getting address with ID: {AddressId}", addressId);
            var address = await addressService.GetAddressByIdAsync(addressId);

            if (address is null)
            {
                logger.LogWarning("Address with ID {AddressId} not found", addressId);
                return NotFound();
            }

            return Ok(MapToAddressDto(address));
        }

        [HttpGet("person/{personId}")]
        public async Task<ActionResult<IEnumerable<AddressDto>>> GetAddressesByPersonId(int personId)
        {
            logger.LogInformation("Getting addresses for person with ID: {PersonId}", personId);

            var person = await personService.GetPersonByIdAsync(personId);
            if (person is null)
            {
                logger.LogWarning("Person with ID {PersonId} not found", personId);
                return NotFound(new { message = "Person not found" });
            }

            var addresses = await addressService.GetAddressesByPersonIdAsync(personId);
            var addressDtos = addresses.Select(MapToAddressDto).ToList();
            return Ok(addressDtos);
        }

        [HttpPost]
        public async Task<ActionResult<AddressDto>> CreateAddress([FromBody] AddressDto addressDto)
        {
            logger.LogInformation("Creating address for person with ID: {PersonId}", addressDto.PersonId);

            var person = await personService.GetPersonByIdAsync(addressDto.PersonId);
            if (person is null)
            {
                logger.LogWarning("Person with ID {PersonId} not found", addressDto.PersonId);
                return NotFound(new { message = "Person not found" });
            }

            try
            {
                var address = MapToAddressEntity(addressDto);
                var createdAddress = await addressService.CreateAddressAsync(address);
                return CreatedAtAction(nameof(GetAddress), new { addressId = createdAddress.AddressId }, MapToAddressDto(createdAddress));
            }
            catch (ArgumentException ex)
            {
                logger.LogWarning("Invalid address data: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{addressId}")]
        public async Task<ActionResult<AddressDto>> UpdateAddress(int addressId, [FromBody] AddressDto addressDto)
        {
            logger.LogInformation("Updating address with ID: {AddressId}", addressId);

            var existingAddress = await addressService.GetAddressByIdAsync(addressId);
            if (existingAddress is null)
            {
                logger.LogWarning("Address with ID {AddressId} not found", addressId);
                return NotFound();
            }

            try
            {
                existingAddress.AddressLine1 = addressDto.AddressLine1;
                existingAddress.AddressLine2 = addressDto.AddressLine2;
                existingAddress.TownOrCity = addressDto.TownOrCity;
                existingAddress.ZipOrPostCode = addressDto.ZipOrPostCode;
                existingAddress.Country = addressDto.Country;
                var updatedAddress = await addressService.UpdateAddressAsync(existingAddress);
                return Ok(MapToAddressDto(updatedAddress));
            }
            catch (ArgumentException ex)
            {
                logger.LogWarning("Invalid address data: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{addressId}")]
        public async Task<IActionResult> DeleteAddress(int addressId)
        {
            logger.LogInformation("Deleting address with ID: {AddressId}", addressId);
            var result = await addressService.DeleteAddressAsync(addressId);

            if (!result)
            {
                logger.LogWarning("Address with ID {AddressId} not found", addressId);
                return NotFound();
            }

            return NoContent();
        }

        //TODO: keep it separate method or auto-mapper
        private static AddressDto MapToAddressDto(Address address)
        {
            return new AddressDto
            {
                AddressId = address.AddressId,
                PersonId = address.PersonId,
                AddressLine1 = address.AddressLine1,
                AddressLine2 = address.AddressLine2,
                TownOrCity = address.TownOrCity,
                ZipOrPostCode = address.ZipOrPostCode,
                Country = address.Country
            };
        }

        private static Address MapToAddressEntity(AddressDto addressDto)
        {
            return new Address
            {
                PersonId = addressDto.PersonId,
                AddressLine1 = addressDto.AddressLine1,
                AddressLine2 = addressDto.AddressLine2,
                TownOrCity = addressDto.TownOrCity,
                ZipOrPostCode = addressDto.ZipOrPostCode,
                Country = addressDto.Country
            };
        }


    }
}
