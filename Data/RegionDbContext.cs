using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace NationRegion.Data
{
  public class RegionDbContext : DbContext
  {
    public DbSet<Region> Region { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      var currentPath = Environment.CurrentDirectory;
      const string projectName = "NationRegion";
      var projectIndex = currentPath.IndexOf(projectName, StringComparison.OrdinalIgnoreCase);
      var dbPath = Path.Combine(currentPath.Substring(0, projectIndex), projectName + "\\Data\\Region.db");
      optionsBuilder.UseSqlite("Data Source=" + dbPath);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      base.OnModelCreating(modelBuilder);

      modelBuilder.Entity<Region>().ToTable("Region").HasKey(c => c.Id);
      modelBuilder.Entity<Region>().Property(c => c.Name).IsRequired().HasMaxLength(64).IsUnicode().IsRequired();
      modelBuilder.Entity<Region>().Property(c => c.Code).HasMaxLength(32).IsUnicode(false).IsRequired();
      modelBuilder.Entity<Region>().Property(c => c.ParentCode).HasMaxLength(32).IsUnicode(false).IsRequired();
      modelBuilder.Entity<Region>().Property(e => e.ZipCode).HasMaxLength(16).IsUnicode(false).IsRequired(false);
      modelBuilder.Entity<Region>().Property(c => c.ChildNodeUrl).HasMaxLength(512).IsUnicode(false);
      modelBuilder.Entity<Region>().Property(c => c.IsGetChild).HasDefaultValue(false);
      modelBuilder.Entity<Region>().Property(c => c.IsDel).HasDefaultValue(false);
      modelBuilder.Entity<Region>().Property(c => c.CreationTime).HasDefaultValueSql("datetime('now')");
    }
  }
}
