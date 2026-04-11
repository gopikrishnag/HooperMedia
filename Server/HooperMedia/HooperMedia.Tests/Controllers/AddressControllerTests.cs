using HooperMedia.Api.Controllers;
using HooperMedia.Api.DTOs;
using HooperMedia.Core.Entities;
using HooperMedia.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;


namespace HooperMedia.Tests.Controllers
{
    public class AddressControllerTests
    {
        private readonly Mock<IAddressService> _addressServiceMock = new();
        private readonly Mock<IPersonService> _personServiceMock = new();
        private readonly Mock<ILogger<AddressController>> _loggerMock = new();

        private AddressController CreateController() =>
            new(_loggerMock.Object, _addressServiceMock.Object, _personServiceMock.Object);

        [Fact]
        public async Task UpdateAddress_ReturnsNotFound_WhenAddressDoesNotExist()
        {
            // Arrange
            var controller = CreateController();
            _addressServiceMock.Setup(s => s.GetAddressByIdAsync(It.IsAny<int>())).ReturnsAsync((Address)null);

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
            _addressServiceMock.Setup(s => s.GetAddressByIdAsync(1)).ReturnsAsync(address);
            _addressServiceMock.Setup(s => s.UpdateAddressAsync(It.IsAny<Address>())).ReturnsAsync(updated);

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