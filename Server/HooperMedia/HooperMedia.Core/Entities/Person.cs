using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace HooperMedia.Core.Entities
{
    public class Person
    {
        public int PersonId { get; set; }
        public required string Name { get; set; }
        public required DateTime DateOfBirth { get; set; }

    }
}
