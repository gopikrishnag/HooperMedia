namespace HooperMedia.Core.Entities
{
    public class Address
    {
        /// Gets or sets the unique identifier for the address (auto-increment).
        public int AddressId { get; set; }

        /// Gets or sets the foreign key to the Person entity.
        public int PersonId { get; set; }
        public required string AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public required string TownOrCity { get; set; }
        public required string ZipOrPostCode { get; set; }
        public required string Country { get; set; }
        public virtual Person? Person { get; set; }
    }
}
