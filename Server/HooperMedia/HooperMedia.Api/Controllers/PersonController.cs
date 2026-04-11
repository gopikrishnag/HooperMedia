using HooperMedia.Api.Controllers.DTOs;
using HooperMedia.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HooperMedia.Api.Controllers
{
    public class PersonController(ILogger<PersonController> logger, IPersonService personService) : Controller
    {
        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PersonDto>>> GetAllPersons()
        {
            logger.LogInformation("Getting all persons");
            var persons = await personService.GetAllPersonsAsync();
            var personDtos = persons.Select(person => new PersonDto
            {
                PersonId = person.PersonId,
                Name = person.Name,
                DateOfBirth = person.DateOfBirth
            }).ToList();
            return Ok(personDtos);
        }

        
    }
}
