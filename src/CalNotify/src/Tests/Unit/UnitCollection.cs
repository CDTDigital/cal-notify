using CalNotifyApi;
using CalNotifyApi.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Tests.Unit
{
    [CollectionDefinition("Unit Tests")]
    public class UnitCollection : ICollectionFixture<UnitTestSetup>
    {
    }

    public class UnitTestSetup : BaseFixture
    {
        public string ConnectionStringKey => "postgresunittest";
        public readonly DbContextOptionsBuilder<BusinessDbContext> Options;

        public UnitTestSetup()
        {

            //Use a PostgreSQL database
            var sqlConnectionString = Configuration.GetConnectionString(ConnectionStringKey);

            Options = new DbContextOptionsBuilder<BusinessDbContext>();
            Options.UseNpgsql(sqlConnectionString, b => b.MigrationsAssembly(Constants.ProjectName));
            
        }

        public BusinessDbContext GetContext()
        {
            return new BusinessDbContext(Options.Options);
        }
    }
}
