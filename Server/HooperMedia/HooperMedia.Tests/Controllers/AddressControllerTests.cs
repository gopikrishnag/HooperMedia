using HooperMedia.Api.Controllers;
using HooperMedia.Api.DTOs;
using HooperMedia.Core.Entities;
using HooperMedia.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace HooperMedia.Tests.Controllers
{
    public class AddressControllerTests
    {
        private readonly IAddressService _addressServiceMock = Substitute.For<IAddressService>();
        private readonly IPersonService _personServiceMock = Substitute.For<IPersonService>();
        private readonly ILogger<AddressController> _loggerMock = Substitute.For<ILogger<AddressController>>();

        private AddressController CreateController() =>
            new(_loggerMock, _addressServiceMock, _personServiceMock);

        [Fact]
        public async Task UpdateAddress_ReturnsNotFound_WhenAddressDoesNotExist()
        {
            // Arrange
            var controller = CreateController();
            _addressServiceMock.GetAddressByIdAsync(Arg.Any<int>()).Returns((Address?)null);

            // Act
            var result = await controller.UpdateAddress(1, new AddressDto
            {
                AddressLine1 = "Test",
                TownOrCity = "Test City",
                ZipOrPostCode = "12345",
                Country = "Test Country"
            });

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public async Task UpdateAddress_ReturnsOk_WhenUpdateIsSuccessful()
        {
            // Arrange
            var controller = CreateController();
            var address = new Address { AddressId = 1, PersonId = 1, AddressLine1 = "A", TownOrCity = "City", ZipOrPostCode = "123", Country = "Country" };
            var updated = new Address { AddressId = 1, PersonId = 1, AddressLine1 = "B", TownOrCity = "City", ZipOrPostCode = "123", Country = "Country" };
            _addressServiceMock.GetAddressByIdAsync(1).Returns(address);
            _addressServiceMock.UpdateAddressAsync(Arg.Any<Address>()).Returns(updated);

            var dto = new AddressDto { AddressId = 1, PersonId = 1, AddressLine1 = "B", TownOrCity = "City", ZipOrPostCode = "123", Country = "Country" };

            // Act
            var result = await controller.UpdateAddress(1, dto);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnDto = Assert.IsType<AddressDto>(okResult.Value);
            Assert.Equal("B", returnDto.AddressLine1);
        }
    }
}