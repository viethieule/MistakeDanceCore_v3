using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Persistence
{
    public class MistakeDanceDbContextFactory : IDesignTimeDbContextFactory<MistakeDanceDbContext>
    {
        private const string CONNECTION_STRING_NAME = "MistakeDanceDatabase";
        private const string ASPNETCORE_ENVIRONMENT = "ASPNETCORE_ENVIRONMENT";
        public MistakeDanceDbContext CreateDbContext(string[] args)
        {
            var basePath = Directory.GetCurrentDirectory() + string.Format("{0}..{0}API", Path.DirectorySeparatorChar);
            return Create(basePath, Environment.GetEnvironmentVariable(ASPNETCORE_ENVIRONMENT));
        }

        private MistakeDanceDbContext Create(string basePath, string environmentName)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            string connectionString = configuration.GetConnectionString(CONNECTION_STRING_NAME);
            var optionsBuilder = new DbContextOptionsBuilder<MistakeDanceDbContext>();

            optionsBuilder.UseSqlServer(connectionString);

            return new MistakeDanceDbContext(optionsBuilder.Options);
        }
    }
}