using app.auth.DataModels;
using Microsoft.EntityFrameworkCore;

namespace app.auth.DataAccess
{
    public class DbClientContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbClientContext(DbContextOptions<DbClientContext> options) : base(options) { }
    }
}
