using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using InsightBoard.Api.DTOs.Notes;
using InsightBoard.Api.Exceptions;
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
                    UserId = "user-id",
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
        
        [Fact]
        public async Task GetAllByUserIdAsync_ShouldReturnUserNotes()
        {
            // Arrange
            var userId = "user-123";
            var notes = new List<Note>
            {
                new Note
                {
                    Id = Guid.NewGuid(),
                    Title = "Note 1",
                    Content = "Content 1",
                    UserId = userId
                },
                new Note
                {
                    Id = Guid.NewGuid(),
                    Title = "Note 2",
                    Content = "Content 2",
                    UserId = userId
                }
            };

            var expectedDtos = notes.Select(n => new NoteDto
            {
                Id = n.Id.ToString(),
                Title = n.Title,
                Content = n.Content
            }).ToList();

            _noteRepoMock.Setup(r => r.GetAllByUserIdAsync(userId))
                .ReturnsAsync(notes);

            _mapperMock.Setup(m => m.Map<IEnumerable<NoteDto>>(notes))
                .Returns(expectedDtos);

            // Act
            var result = await _noteService.GetAllByUserIdAsync(userId);

            // Assert
            result.Should().NotBeNull();
            result.Should().HaveCount(2);
            
            // Verify
            _noteRepoMock.Verify(r => r.GetAllByUserIdAsync(userId), Times.Once);
        }
        
        [Fact]
        public async Task CreateAsync_ShouldCreateNote()
        {
            // Arrange
            var userId = "user-123";
            var request = new CreateNoteRequest
            {
                Title = "New Note",
                Content = "New Content"
            };

            var createdNote = new Note
            {
                Id = Guid.NewGuid(),
                Title = request.Title,
                Content = request.Content,
                UserId = userId,
                CreatedAt = DateTime.UtcNow
            };

            var expectedDto = new NoteDto
            {
                Id = createdNote.Id.ToString(),
                Title = createdNote.Title,
                Content = createdNote.Content,
                CreatedAt = createdNote.CreatedAt
            };

            _noteRepoMock.Setup(r => r.CreateAsync(It.IsAny<Note>()))
                .Callback<Note>(note => 
                {
                    note.Id = createdNote.Id;
                    note.CreatedAt = createdNote.CreatedAt;
                })
                .Returns(Task.CompletedTask);

            _mapperMock.Setup(m => m.Map<NoteDto>(It.IsAny<Note>()))
                .Returns(expectedDto);

            // Act
            var result = await _noteService.CreateAsync(request, userId);

            // Assert
            result.Should().NotBeNull();
            result.Title.Should().Be(request.Title);
            result.Content.Should().Be(request.Content);
            
            // Verify
            _noteRepoMock.Verify(r => r.CreateAsync(It.IsAny<Note>()), Times.Once);
        }

        [Fact]
        public async Task UpdateNoteAsync_ShouldUpdateNote_WhenUserIsAuthorized()
        {
            // Arrange
            var noteId = Guid.NewGuid();
            var userId = "user-123";
            
            var note = new Note
            {
                Id = noteId,
                Title = "Old Title",
                Content = "Old Content",
                UserId = userId
            };
            
            var request = new UpdateNoteRequest
            {
                Title = "Updated Title",
                Content = "Updated Content"
            };

            _noteRepoMock.Setup(r => r.GetByIdAsync(noteId))
                .ReturnsAsync(note);

            _noteRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Note>()))
                .Returns(Task.CompletedTask);

            // Act
            await _noteService.UpdateNoteAsync(noteId.ToString(), request, userId);

            // Assert
            note.Title.Should().Be(request.Title);
            note.Content.Should().Be(request.Content);
            
            // Verify
            _noteRepoMock.Verify(r => r.GetByIdAsync(noteId), Times.Once);
            _noteRepoMock.Verify(r => r.UpdateAsync(note), Times.Once);
        }

        [Fact]
        public async Task UpdateNoteAsync_ShouldThrowUnauthorized_WhenUserIsNotAuthor()
        {
            // Arrange
            var noteId = Guid.NewGuid();
            var noteAuthorId = "author-123";
            var differentUserId = "different-user-456";
            
            var note = new Note
            {
                Id = noteId,
                Title = "Old Title",
                Content = "Old Content",
                UserId = noteAuthorId
            };
            
            var request = new UpdateNoteRequest
            {
                Title = "Updated Title",
                Content = "Updated Content"
            };

            _noteRepoMock.Setup(r => r.GetByIdAsync(noteId))
                .ReturnsAsync(note);

            // Act & Assert
            await Assert.ThrowsAsync<UnauthorizedException>(() => 
                _noteService.UpdateNoteAsync(noteId.ToString(), request, differentUserId));
            
            // Verify
            _noteRepoMock.Verify(r => r.GetByIdAsync(noteId), Times.Once);
            _noteRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Note>()), Times.Never);
        }

        [Fact]
        public async Task UpdateNoteAsync_ShouldThrowNotFound_WhenNoteDoesNotExist()
        {
            // Arrange
            var noteId = Guid.NewGuid();
            var userId = "user-123";
            
            var request = new UpdateNoteRequest
            {
                Title = "Updated Title",
                Content = "Updated Content"
            };

            _noteRepoMock.Setup(r => r.GetByIdAsync(noteId))
                .ReturnsAsync((Note)null);

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => 
                _noteService.UpdateNoteAsync(noteId.ToString(), request, userId));
            
            // Verify
            _noteRepoMock.Verify(r => r.GetByIdAsync(noteId), Times.Once);
            _noteRepoMock.Verify(r => r.UpdateAsync(It.IsAny<Note>()), Times.Never);
        }

        [Fact]
        public async Task DeleteNoteAsync_ShouldDeleteNote_WhenUserIsAuthorized()
        {
            // Arrange
            var noteId = Guid.NewGuid();
            var userId = "user-123";
            
            var note = new Note
            {
                Id = noteId,
                Title = "Note Title",
                Content = "Note Content",
                UserId = userId
            };

            _noteRepoMock.Setup(r => r.GetByIdAsync(noteId))
                .ReturnsAsync(note);

            _noteRepoMock.Setup(r => r.DeleteAsync(It.IsAny<Note>()))
                .Returns(Task.CompletedTask);

            // Act
            await _noteService.DeleteNoteAsync(noteId.ToString(), userId);

            // Verify
            _noteRepoMock.Verify(r => r.GetByIdAsync(noteId), Times.Once);
            _noteRepoMock.Verify(r => r.DeleteAsync(note), Times.Once);
        }

        [Fact]
        public async Task PublishNoteAsync_ShouldSetNotePublic_WhenUserIsAuthorized()
        {
            // Arrange
            var noteId = Guid.NewGuid();
            var userId = "user-123";
            
            var note = new Note
            {
                Id = noteId,
                Title = "Note Title",
                Content = "Note Content",
                UserId = userId,
                IsPublic = false
            };

            _noteRepoMock.Setup(r => r.GetByIdAsync(noteId))
                .ReturnsAsync(note);

            _noteRepoMock.Setup(r => r.UpdateAsync(It.IsAny<Note>()))
                .Returns(Task.CompletedTask);

            // Act
            await _noteService.PublishNoteAsync(noteId.ToString(), userId);

            // Assert
            note.IsPublic.Should().BeTrue();
            
            // Verify
            _noteRepoMock.Verify(r => r.GetByIdAsync(noteId), Times.Once);
            _noteRepoMock.Verify(r => r.UpdateAsync(note), Times.Once);
        }

        [Fact]
        public async Task GetPublicNotesPagedAsync_ShouldReturnPagedNotes()
        {
            // Arrange
            int pageNumber = 1;
            int pageSize = 10;
            string sortBy = "created_at";
            
            var notes = new List<Note>
            {
                new Note { Id = Guid.NewGuid(), Title = "Note 1", IsPublic = true },
                new Note { Id = Guid.NewGuid(), Title = "Note 2", IsPublic = true }
            };
            
            int totalRecords = 15;
            
            var expectedDtos = notes.Select(n => new NoteDto
            {
                Id = n.Id.ToString(),
                Title = n.Title,
            }).ToList();

            _noteRepoMock.Setup(r => r.GetPublicNotesPagedAsync(pageNumber, pageSize, sortBy))
                .ReturnsAsync((notes, totalRecords));

            _mapperMock.Setup(m => m.Map<IEnumerable<NoteDto>>(notes))
                .Returns(expectedDtos);

            // Act
            var result = await _noteService.GetPublicNotesPagedAsync(pageNumber, pageSize, sortBy);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().HaveCount(2);
            result.PageNumber.Should().Be(pageNumber);
            result.PageSize.Should().Be(pageSize);
            result.TotalRecords.Should().Be(totalRecords);
            
            // Verify
            _noteRepoMock.Verify(r => r.GetPublicNotesPagedAsync(pageNumber, pageSize, sortBy), Times.Once);
        }
    }
}