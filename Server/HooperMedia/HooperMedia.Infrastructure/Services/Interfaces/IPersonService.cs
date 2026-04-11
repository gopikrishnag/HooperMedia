using HooperMedia.Core.Entities;

namespace HooperMedia.Infrastructure.Services.Interfaces
{
    public interface IPersonService
    {
        Task<Person?> GetPersonByIdAsync(int personId);
        Task<IEnumerable<Person>> GetAllPersonsAsync();
    }
}
