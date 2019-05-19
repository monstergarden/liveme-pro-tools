using System.Linq;
using LMPT.Core.Contract.DB;
using LMPT.Core.Contract.DB.Bookmark;
using LMPT.Core.Contract.DB.Cache;
using Microsoft.EntityFrameworkCore;

namespace LMPT.DB
{
    public class ApplicationDBContext : DbContext
    {
        private readonly ConfigurationProvider config;

        public ApplicationDBContext()
        {
        }

        public ApplicationDBContext(ConfigurationProvider config)
        {
            this.config = config;
        }

        public DbSet<Replay> Replays { get; set; }
        public DbSet<Bookmark> Bookmarks { get; set; }
        public DbSet<ProfileSeen> ProfileSeen { get; set; }
        public DbSet<ScanResult> LastScanResult { get; set; }
        public DbSet<LivemeAuthentication> LivemeAuthentication { get; set; }
        public DbSet<User> User { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasIndex(r => r.UId);
            modelBuilder.Entity<User>().HasKey(r => r.InternalId);
            modelBuilder.Entity<Replay>().HasKey(r => r.VId);
            modelBuilder.Entity<Bookmark>().HasKey(b => b.Uid);
            modelBuilder.Entity<ProfileSeen>().HasKey(b => b.Uid);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (config != null)
                optionsBuilder.UseSqlite("Data Source=" + config.DbFile);
            else
                optionsBuilder.UseSqlite("Data Source=deleteMe.db");
        }


        public User StoreUser(string uid, string name)
        {
            var found = User.FirstOrDefault(x => x.UId.Equals(uid));
            if (found == null)
            {
                var newUser = new User
                {
                    UId = uid,
                    Name = name
                };
                Add(newUser);
                SaveChanges();

                return newUser;
            }

            found.Name = name;

            return found;
        }
    }
}