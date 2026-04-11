namespace HooperMedia.Api.DTOs
{
    public class PersonDto
    {
        public int PersonId { get; set; }
        public required string Name { get; set; }
        public required DateTime DateOfBirth { get; set; }
    }
}
