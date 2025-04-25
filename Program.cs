using InsightBoard.Api.Data;
using InsightBoard.Api.Services.Notes;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<InsightBoardDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddScoped<INoteService, NoteService>();

var app = builder.Build();

app.UseAuthorization();
app.MapControllers();

app.Run();