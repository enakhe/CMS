namespace CMS.Domain.Entities
{
    public class Criminal
    {
        public required string Id { get; set; } = Guid.NewGuid().ToString();
    }
}
