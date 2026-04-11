using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using HooperMedia.Core.Entities;

namespace HooperMedia.Infrastructure.Repositories.Interfaces
{
    public interface IAddressRepository : IRepository<Address>
    {
        Task<IEnumerable<Address>> GetByPersonIdAsync(int personId);
    }
}
