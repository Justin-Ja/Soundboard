using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Soundboard.Domain.DataAccess.Implementations;

namespace Soundboard.Domain.DataAccess;

public class SoundboardDbContext : DbContext
{
    public DbSet<SoundButton> SoundButtons { get; set; }
    public DbSet<SoundButtonGridLayout> GridLayouts { get; set; }

    public SoundboardDbContext(DbContextOptions<SoundboardDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SoundButton>(entity =>
        {
            entity.HasKey(e => e.Guid);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(255);
            entity.Property(e => e.FilePath).IsRequired();
            entity.HasOne(e => e.GridLayout)
                  .WithMany(c => c.SoundButtons)
                  .HasForeignKey(e => e.GridId);
        });

        modelBuilder.Entity<SoundButtonGridLayout>(entity =>
        {
            entity.HasKey(e => e.Guid);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(127);

            entity.HasMany(e => e.SoundButtons)
              .WithOne(s => s.GridLayout)
              .HasForeignKey(s => s.GridId);
        });
    }
}

public class SoundboardDbContextFactory : IDesignTimeDbContextFactory<SoundboardDbContext>
{
    public SoundboardDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<SoundboardDbContext>();
        optionsBuilder.UseSqlite("Data Source=soundboard.db");
        return new SoundboardDbContext(optionsBuilder.Options);
    }
}