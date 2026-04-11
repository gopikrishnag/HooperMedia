using HooperMedia.Core.Entities;
using HooperMedia.Infrastructure.Data;
using HooperMedia.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HooperMedia.Infrastructure.Repositories
{
    public class AddressRepository(ApplicationDbContext context) : Repository<Address>(context), IAddressRepository
    {
        public async Task<IEnumerable<Address>> GetByPersonIdAsync(int personId)
        {
            var addresses = await context.Addresses
                .Where(a => a.PersonId == personId)
                .ToListAsync();
            return addresses;
        }
    }
}
