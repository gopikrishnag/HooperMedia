using HooperMedia.Core.Entities;
using HooperMedia.Infrastructure.Repositories.Interfaces;
using HooperMedia.Infrastructure.Services.Interfaces;

namespace HooperMedia.Infrastructure.Services
{
    public class PersonService(IPersonRepository personRepository) : IPersonService
    {

        public async Task<Person?> GetPersonByIdAsync(int personId)
        {
            return await personRepository.GetByIdAsync(personId);
        }

        public async Task<IEnumerable<Person>> GetAllPersonsAsync()
        {
            return await personRepository.GetAllAsync();
        }
    }
}
