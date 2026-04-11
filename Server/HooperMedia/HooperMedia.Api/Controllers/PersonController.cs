using HooperMedia.Api.Controllers.DTOs;
using HooperMedia.Core.Entities;
using HooperMedia.Infrastructure.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HooperMedia.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PersonController(ILogger<PersonController> logger, IPersonService personService) : ControllerBase
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


        [HttpGet("{personId}")]
        public async Task<ActionResult<PersonDto>> GetPerson(int personId)
        {
            logger.LogInformation("Getting person with ID: {PersonId}", personId);
            var person = await personService.GetPersonByIdAsync(personId);

            if (person is null)
            {
                logger.LogWarning("Person with ID {PersonId} not found", personId);
                return NotFound();
            }

            return Ok(new PersonDto
            {
                PersonId = person.PersonId,
                Name = person.Name,
                DateOfBirth = person.DateOfBirth
            });
        }

        [HttpPost]
        public async Task<ActionResult<PersonDto>> CreatePerson([FromBody] PersonDto personDto)
        {
            logger.LogInformation("Creating person with name: {Name}", personDto.Name);

            try
            {
                var person = new Person
                {
                    Name = personDto.Name,
                    DateOfBirth = personDto.DateOfBirth
                };

                var createdPerson = await personService.CreatePersonAsync(person);
                var createdPersonDto = new PersonDto
                {
                    PersonId = createdPerson.PersonId,
                    Name = createdPerson.Name,
                    DateOfBirth = createdPerson.DateOfBirth
                };
                return CreatedAtAction(nameof(GetPerson), new { personId = createdPerson.PersonId }, createdPersonDto);
            }
            catch (ArgumentException ex)
            {
                logger.LogWarning(ex, "Invalid person data: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{personId}")]
        public async Task<IActionResult> DeletePerson(int personId)
        {
            logger.LogInformation("Deleting person with ID: {PersonId}", personId);
            var result = await personService.DeletePersonAsync(personId);

            if (!result)
            {
                logger.LogWarning("Person with ID {PersonId} not found", personId);
                return NotFound();
            }

            return NoContent();
        }

        [HttpPut("{personId}")]
        public async Task<ActionResult<PersonDto>> UpdatePerson(int personId, [FromBody] PersonDto personDto)
        {
            logger.LogInformation("Updating person with ID: {PersonId}", personId);

            var existingPerson = await personService.GetPersonByIdAsync(personId);
            if (existingPerson is null)
            {
                logger.LogWarning("Person with ID {PersonId} not found", personId);
                return NotFound();
            }

            try
            {
                existingPerson.Name = personDto.Name;
                existingPerson.DateOfBirth = personDto.DateOfBirth;
                var updatedPerson = await personService.UpdatePersonAsync(existingPerson);
               var updatedPersonDto = new PersonDto
                {
                    PersonId = updatedPerson.PersonId,
                    Name = updatedPerson.Name,
                    DateOfBirth = updatedPerson.DateOfBirth
                };
                return Ok(updatedPersonDto);
            }
            catch (ArgumentException ex)
            {
                logger.LogWarning(ex, "Invalid person data: {Message}", ex.Message);
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
