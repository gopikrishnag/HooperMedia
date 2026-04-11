using HooperMedia.Core.Constants;
using HooperMedia.Core.BusinessRules;
using HooperMedia.Core.Entities;
using HooperMedia.Core.Extensions;
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

        public async Task<Person> CreatePersonAsync(Person person)
        {
            ValidatePerson(person);
            return await personRepository.AddAsync(person);
        }
        private static void ValidatePerson(Person person)
        {
            if (string.IsNullOrWhiteSpace(person.Name) || person.Name.Length > PersonBusinessRules.NameMaxLength)
            {
                throw new ArgumentException(ValidationErrorMessages.PersonNameInvalid);
            }

            if (!person.DateOfBirth.IsValidDateOfBirth())
            {
                throw new ArgumentException(ValidationErrorMessages.PersonDateOfBirthInvalid);
            }
        }

        public async Task<bool> DeletePersonAsync(int personId)
        {
            return await personRepository.DeleteAsync(personId);
        }
        public async Task<Person> UpdatePersonAsync(Person person)
        {
            ValidatePerson(person);
            return await personRepository.UpdateAsync(person);
        }
    }
}
