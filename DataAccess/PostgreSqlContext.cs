using Microsoft.EntityFrameworkCore;
using personalweb.Models;

namespace personalweb.DataAccess
{
    public class PostgreSqlContext: DbContext
    {
        public DbSet<SiteCount> SiteCounts => Set<SiteCount>();

        public PostgreSqlContext(DbContextOptions<PostgreSqlContext> options) : base(options)
        {
        }

        public override int SaveChanges()
        {
            ChangeTracker.DetectChanges();
            return base.SaveChanges();
        }
    }
}