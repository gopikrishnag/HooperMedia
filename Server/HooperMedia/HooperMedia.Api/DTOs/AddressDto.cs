namespace HooperMedia.Api.DTOs
{
    public class AddressDto
    {
        public int AddressId { get; set; }
        public int PersonId { get; set; }
        public required string AddressLine1 { get; set; }
        public string? AddressLine2 { get; set; }
        public required string TownOrCity { get; set; }
        public required string ZipOrPostCode { get; set; }
        public required string Country { get; set; }
    }
}
