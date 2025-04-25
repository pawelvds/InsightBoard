namespace InsightBoard.Api.Models
{
    public class Answer
    {
        public Guid Id { get; set; } 
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid QuestionId { get; set; }
        public Question? Question { get; set; }

        public string AuthorId { get; set; } = string.Empty;
        public User? Author { get; set; }
    }
}