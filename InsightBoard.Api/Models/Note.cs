namespace InsightBoard.Api.Models
{
    public class Note
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public string AuthorId { get; set; } = string.Empty;
        public User Author { get; set; }
        public ICollection<Question> Questions { get; set; } = new List<Question>();
        public bool IsPublic { get; set; } = false;
    }
}