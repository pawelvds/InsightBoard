namespace InsightBoard.Api.Models
{
    public class Question
    {
        public Guid Id { get; set; } // Klucz główny
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Relacja do Note (pytanie zadane do notatki)
        public Guid NoteId { get; set; }
        public Note? Note { get; set; }

        // Relacja do autora pytania
        public string AuthorId { get; set; } = string.Empty;
        public User? Author { get; set; }

        public ICollection<Answer> Answers { get; set; } = new List<Answer>();
    }
}
