using AspNetCoreTemplate.Models;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreTemplate.Data;

public sealed class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<TodoItem> TodoItems => Set<TodoItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TodoItem>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Title).HasMaxLength(200);
            entity.Property(x => x.CreatedAtUtc);
            entity.Property(x => x.IsCompleted);
        });
    }
}
