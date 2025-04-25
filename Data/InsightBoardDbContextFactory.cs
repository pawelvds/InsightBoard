using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace InsightBoard.Api.Data
{
    public class InsightBoardDbContextFactory : IDesignTimeDbContextFactory<InsightBoardDbContext>
    {
        public InsightBoardDbContext CreateDbContext(string[] args)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder = new DbContextOptionsBuilder<InsightBoardDbContext>();
            var connectionString = config.GetConnectionString("DefaultConnection");

            optionsBuilder.UseNpgsql(connectionString);

            return new InsightBoardDbContext(optionsBuilder.Options);
        }
    }
}