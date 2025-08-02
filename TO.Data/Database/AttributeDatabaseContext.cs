using Microsoft.EntityFrameworkCore;
using TO.Commons.Configs;
using Godot;
using TO.Data.Converters;
using TO.Data.DTO.GameAbilitySystem.GameplayAttribute;

namespace TO.Data.Database;

public class AttributeDatabaseContext : DbContext
{
    public DbSet<AttributeSetDTO> AttributeSets { get; set; }
    public DbSet<BasicAttributeValueDTO> BasicAttributeValues { get; set; }
    public DbSet<ShipAttributeValueDTO> ShipAttributeValues { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        string dbPath = ProjectSettings.GlobalizePath(ConstConfigs.AttributeDatabasePath);
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AttributeSetDTO>().ToTable("AttributeSets");
        modelBuilder.Entity<BasicAttributeValueDTO>().ToTable("AttributeValues_BasicAttributes");
        modelBuilder.Entity<ShipAttributeValueDTO>().ToTable("AttributeValues_ShipAttributes");

        modelBuilder.Entity<BasicAttributeValueDTO>().Property(e => e.AttributeType)
            .HasConversion(new AttributeDefinitionConverter());
        modelBuilder.Entity<ShipAttributeValueDTO>().Property(e => e.AttributeType)
            .HasConversion(new AttributeDefinitionConverter());

          
    }
}