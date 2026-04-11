using HooperMedia.Core.Entities;

namespace HooperMedia.Infrastructure.Services.Interfaces
{
    public interface IPersonService
    {
        Task<Person?> GetPersonByIdAsync(int personId);
        Task<IEnumerable<Person>> GetAllPersonsAsync();
        Task<Person> CreatePersonAsync(Person person);
        Task<bool> DeletePersonAsync(int personId);
        Task<Person> UpdatePersonAsync(Person person);
    }
}
