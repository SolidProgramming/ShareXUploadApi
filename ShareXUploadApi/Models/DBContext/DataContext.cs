using Microsoft.EntityFrameworkCore;

namespace ShareXUploadApi.Models.DBContext
{
    public class DataContext : DbContext
    {
        protected readonly IConfiguration Configuration;

        public DataContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // connect to mysql with connection string from app settings
            string connectionString = DBService.ConnectionString;
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
        }

        public DbSet<FileModel> File { get; set; }
    }
}
