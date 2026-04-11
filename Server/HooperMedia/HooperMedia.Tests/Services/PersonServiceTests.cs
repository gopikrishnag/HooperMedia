using Moq;
using HooperMedia.Core.Entities;
using HooperMedia.Infrastructure.Repositories.Interfaces;
using HooperMedia.Infrastructure.Services;

namespace HooperMedia.Tests.Services
{
    public class PersonServiceTests
    {
        private readonly Mock<IPersonRepository> personRepositoryMock;
        private readonly PersonService personService;

        public PersonServiceTests()
        {
            personRepositoryMock = new Mock<IPersonRepository>();
            personService = new PersonService(personRepositoryMock.Object);
        }

        [Fact]
        public async Task GetPersonByIdAsync_WithValidId_ReturnsPersonDto()
        {
            var personId = 1;
            var personName = "Gopi Govind";
            var expectedPerson = new Person { PersonId = personId, Name = personName, DateOfBirth = DateTime.UtcNow.AddYears(-30) };

            personRepositoryMock.Setup(r => r.GetByIdAsync(personId))
                .ReturnsAsync(expectedPerson);

            var result = await personService.GetPersonByIdAsync(personId);

            Assert.NotNull(result);
            Assert.Equal(personId, result.PersonId);
            Assert.Equal(personName, result.Name);
            personRepositoryMock.Verify(r => r.GetByIdAsync(personId), Times.Once);
        }

        [Fact]
        public async Task GetPersonByIdAsync_WithInvalidId_ReturnsNull()
        {
            var personId = 999;

            personRepositoryMock.Setup(r => r.GetByIdAsync(personId))
                .ReturnsAsync((Person?)null);

            var result = await personService.GetPersonByIdAsync(personId);

            Assert.Null(result);
            personRepositoryMock.Verify(r => r.GetByIdAsync(personId), Times.Once);
        }

        [Fact]
        public async Task GetAllPersonsAsync_ReturnsListOfPersons()
        {
            var expectedPersons = new List<Person>
        {
            new() { PersonId = 1, Name = "John David", DateOfBirth = DateTime.UtcNow.AddYears(-30) },
            new() { PersonId = 2, Name = "Jane Smith", DateOfBirth = DateTime.UtcNow.AddYears(-25) }
        };

            personRepositoryMock.Setup(r => r.GetAllAsync())
                .ReturnsAsync(expectedPersons);

            var result = await personService.GetAllPersonsAsync();

            Assert.NotNull(result);
            Assert.Equal(2, result.Count());
            personRepositoryMock.Verify(r => r.GetAllAsync(), Times.Once);
        }

        [Fact]
        public async Task CreatePersonAsync_WithValidData_CreatesPerson()
        {
            var newPerson = new Person { Name = "New Person", DateOfBirth = DateTime.UtcNow.AddYears(-30) };
            var createdPerson = new Person { PersonId = 1, Name = newPerson.Name, DateOfBirth = newPerson.DateOfBirth };

            personRepositoryMock.Setup(r => r.AddAsync(newPerson))
                .ReturnsAsync(createdPerson);

            var result = await personService.CreatePersonAsync(newPerson);

            Assert.NotNull(result);
            Assert.Equal(1, result.PersonId);
            Assert.Equal("New Person", result.Name);
            personRepositoryMock.Verify(r => r.AddAsync(newPerson), Times.Once);
        }

        [Fact]
        public async Task CreatePersonAsync_WithInvalidName_ThrowsException()
        {
            var invalidPerson = new Person { Name = "", DateOfBirth = DateTime.UtcNow.AddYears(-30) };

            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                personService.CreatePersonAsync(invalidPerson));

            Assert.Contains("Person name must not be null", exception.Message);
        }

        [Fact]
        public async Task CreatePersonAsync_WithNameExceeding50Chars_ThrowsException()
        {
            var invalidPerson = new Person
            {
                Name = new string('A', 51),
                DateOfBirth = DateTime.UtcNow.AddYears(-30)
            };

            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                personService.CreatePersonAsync(invalidPerson));

            Assert.Contains("must not exceed 50 characters", exception.Message);
        }

        [Fact]
        public async Task CreatePersonAsync_WithFutureDate_ThrowsException()
        {
            var invalidPerson = new Person
            {
                Name = "Future Person",
                DateOfBirth = DateTime.UtcNow.AddDays(1)
            };

            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                personService.CreatePersonAsync(invalidPerson));

            Assert.Contains("less than today", exception.Message);
        }

        [Fact]
        public async Task CreatePersonAsync_WithAgeOver100_ThrowsException()
        {
            var invalidPerson = new Person
            {
                Name = "Old Person",
                DateOfBirth = DateTime.UtcNow.AddYears(-101)
            };

            var exception = await Assert.ThrowsAsync<ArgumentException>(() =>
                personService.CreatePersonAsync(invalidPerson));

            Assert.Contains("cannot be older than 100 years", exception.Message);
        }

        [Fact]
        public async Task UpdatePersonAsync_WithValidData_UpdatesPerson()
        {
            var personToUpdate = new Person { PersonId = 1, Name = "Updated Name", DateOfBirth = DateTime.UtcNow.AddYears(-30) };

            personRepositoryMock.Setup(r => r.UpdateAsync(personToUpdate))
                .ReturnsAsync(personToUpdate);

            var result = await personService.UpdatePersonAsync(personToUpdate);

            Assert.NotNull(result);
            Assert.Equal("Updated Name", result.Name);
            personRepositoryMock.Verify(r => r.UpdateAsync(personToUpdate), Times.Once);
        }

        [Fact]
        public async Task DeletePersonAsync_WithValidId_DeletesPerson()
        {
            var personId = 1;

            personRepositoryMock.Setup(r => r.DeleteAsync(personId))
                .ReturnsAsync(true);

            var result = await personService.DeletePersonAsync(personId);

            Assert.True(result);
            personRepositoryMock.Verify(r => r.DeleteAsync(personId), Times.Once);
        }

        [Fact]
        public async Task DeletePersonAsync_WithInvalidId_ReturnsFalse()
        {
            var personId = 999;

            personRepositoryMock.Setup(r => r.DeleteAsync(personId))
                .ReturnsAsync(false);

            var result = await personService.DeletePersonAsync(personId);

            Assert.False(result);
            personRepositoryMock.Verify(r => r.DeleteAsync(personId), Times.Once);
        }

    }
}