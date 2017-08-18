namespace Apex.Services.Models
{
    public sealed class IdNameDto
    {
        public IdNameDto()
        {
        }

        public IdNameDto(int id, string name)
        {
            Id = id;
            Name = name;
        }

        public int Id { get; set; }

        public string Name { get; set; }
    }
}
