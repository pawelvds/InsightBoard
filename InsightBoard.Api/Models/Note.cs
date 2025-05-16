﻿namespace InsightBoard.Api.Models
{
    public class Note
    {
        public Guid Id { get; set; }

        public string Title { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string UserId { get; set; } = string.Empty;

        public User User { get; set; } = null!;

        public ICollection<Question> Questions { get; set; } = new List<Question>();

        public bool IsPublic { get; set; } = false;
    }
}