using HooperMedia.Core.Entities;
using HooperMedia.Infrastructure.Data;
using HooperMedia.Infrastructure.Repositories.Interfaces;

namespace HooperMedia.Infrastructure.Repositories
{
    public class PersonRepository(ApplicationDbContext context) : Repository<Person>(context), IPersonRepository
    {

    }
}
