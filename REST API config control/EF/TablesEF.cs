using Microsoft.EntityFrameworkCore;

namespace REST_API_config_control.EF
{
    public class ConfigEF
    {
        public int row_id { get; }
        public DateTime date { get; set; }
        public string user_name { get; set; }
        public string config_name { get; set; }
        public int version { get; set; }
        public string key { get; set; }
        public string value { get; set; }
    }
    public class SubscribeEF
    {
        public int row_id { get; }
        public DateTime date { get; set; }
        public string login { get; set; }
        public string password { get; set; }
        public int subscribeTypeKey { get; set; }
        public string? savedMesseges { get; set; }
    }

    public class ApplicationContext : DbContext
    {
        public DbSet<ConfigEF> Configs => Set<ConfigEF>();
        public DbSet<SubscribeEF> Subscribes => Set<SubscribeEF>();
        public ApplicationContext() => Database.EnsureCreated();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ConfigEF>()
            .HasIndex(p => new { p.user_name, p.config_name, p.version }).IsUnique();

            modelBuilder.Entity<ConfigEF>()
            .Property(e => e.row_id)
            .ValueGeneratedOnAdd();

            modelBuilder.Entity<ConfigEF>()
            .HasKey(t => t.row_id);

            modelBuilder.Entity<SubscribeEF>()
            .HasIndex(p => new { p.login, p.password }).IsUnique();

            modelBuilder.Entity<SubscribeEF>()
            .Property(e => e.row_id)
            .ValueGeneratedOnAdd();

            modelBuilder.Entity<SubscribeEF>()
            .HasKey(t => t.row_id);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=appdb.db");
        }
    }
}
