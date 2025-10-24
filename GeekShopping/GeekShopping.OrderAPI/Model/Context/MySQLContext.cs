using Microsoft.EntityFrameworkCore;

namespace GeekShopping.OrderAPI.Model.Context
{
    public class MySQLContext : DbContext
    {
        public MySQLContext() { }
        public MySQLContext(DbContextOptions<MySQLContext> options) : base(options) {}

        public DbSet<OrderDetail> Details { get; set; }
        public DbSet<OrderHeader> Headers { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(MySQLContext).Assembly);
        }
    }
}
