using TaskManager.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace TaskManager.Api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<TaskItem> TaskItems {get; set;}
        public DbSet<TaskTag> TaskTags { get; set; }
        public DbSet<Tag> Tags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TaskItem>().ToTable("TaskItems");
            modelBuilder.Entity<TaskTag>().ToTable("TaskTags");
            modelBuilder.Entity<Tag>().ToTable("Tags");


            //PK
            modelBuilder.Entity<TaskItem>()
            .HasKey(task => task.Id);
            modelBuilder.Entity<Tag>()
            .HasKey(tag => tag.Id);
            modelBuilder.Entity<TaskTag>()
            .HasKey(tt => new {tt.TaskItemId, tt.TagId});

            //Relationships
            modelBuilder.Entity<TaskTag>()
            .HasOne(tt => tt.TaskItem)
            .WithMany(task => task.TaskTags)
            .HasForeignKey(tt => tt.TaskItemId)
            .HasConstraintName("FK_TaskTags_TaskItems");

            modelBuilder.Entity<TaskTag>()
            .HasOne(tt => tt.Tag)
            .WithMany(tag => tag.TaskTags)
            .HasForeignKey(tt => tt.TagId)
            .HasConstraintName("FK_TaskTags_Tags");

            //Indexes
            modelBuilder.Entity<TaskTag>()
            .HasIndex(tt => tt.TaskItemId)
            .HasDatabaseName("IX_TaskTags_TaskItemId");

            modelBuilder.Entity<TaskTag>()
            .HasIndex(tt => tt.TagId)
            .HasDatabaseName("IX_TaskTags_TagId");
        }
    }
}