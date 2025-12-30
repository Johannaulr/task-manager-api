using TaskManager.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace TaskManager.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<TaskItem> taskItems {get; set;}
    }
}