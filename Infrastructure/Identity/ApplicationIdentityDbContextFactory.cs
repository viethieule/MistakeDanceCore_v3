using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Identity
{
    public class ApplicationIdentityDbContextFactory : IDesignTimeDbContextFactory<ApplicationIdentityDbContext>
    {
        private const string CONNECTION_STRING_NAME = "Identity";
        private const string ASPNETCORE_ENVIRONMENT = "ASPNETCORE_ENVIRONMENT";
        public ApplicationIdentityDbContext CreateDbContext(string[] args)
        {
            var basePath = Directory.GetCurrentDirectory() + string.Format("{0}..{0}API", Path.DirectorySeparatorChar);
            return Create(basePath, Environment.GetEnvironmentVariable(ASPNETCORE_ENVIRONMENT));
        }

        private ApplicationIdentityDbContext Create(string basePath, string environmentName)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.{environmentName}.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            string connectionString = configuration.GetConnectionString(CONNECTION_STRING_NAME);
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationIdentityDbContext>();

            optionsBuilder.UseSqlServer(connectionString);

            return new ApplicationIdentityDbContext(optionsBuilder.Options);
        }
    }
}