using LMPT.DB;

namespace LMPT.Core.Services.Data
{
    public class DbContextFactory
    {
        private readonly ConfigurationProvider _config;

        public DbContextFactory(ConfigurationProvider config)
        {
            _config = config;
        }

        public ApplicationDBContext CreateApplicationDbContext()
        {
            return new ApplicationDBContext(_config);
        }
    }
}