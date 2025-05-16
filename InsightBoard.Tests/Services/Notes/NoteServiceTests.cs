using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using InsightBoard.Api.DTOs.Notes;
using InsightBoard.Api.Models;
using InsightBoard.Api.Repositories;
using InsightBoard.Api.Services.Notes;
using Moq;
using Xunit;

namespace InsightBoard.Tests.Services.Notes
{
    public class NoteServiceTests
    {
        private readonly NoteService _noteService;
        private readonly Mock<INoteRepository> _noteRepoMock = new Mock<INoteRepository>();
        private readonly Mock<IMapper> _mapperMock = new Mock<IMapper>();

        public NoteServiceTests()
        {
            _noteService = new NoteService(_noteRepoMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetPublicNotesByUsernameAsync_ShouldReturnNotes_WhenUserExists()
        {
            // Arrange
            var username = "testuser";
            
            var notes = new List<Note>
            {
                new Note
                {
                    Id = Guid.NewGuid(),
                    Title = "Note 1",
                    Content = "Some public content",
                    CreatedAt = DateTime.UtcNow,
                    AuthorId = "user-id",
                    IsPublic = true
                }
            };

            _noteRepoMock.Setup(r => r.GetPublicNotesByUsernameAsync(username))
                .ReturnsAsync(notes);

            var expectedDtos = notes.Select(n => new NoteDto
            {
                Id = n.Id.ToString(),
                Title = n.Title,
                Content = n.Content,
                CreatedAt = n.CreatedAt,
                IsPublic = n.IsPublic
            }).ToList();

            _mapperMock.Setup(m => m.Map<IEnumerable<NoteDto>>(It.IsAny<IEnumerable<Note>>()))
                .Returns(expectedDtos);

            // Act
            var result = await _noteService.GetPublicNotesByUsernameAsync(username);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(1);
            result.First().Title.Should().Be("Note 1");
            
            // Verify
            _noteRepoMock.Verify(r => r.GetPublicNotesByUsernameAsync(username), Times.Once);
        }
    }
}