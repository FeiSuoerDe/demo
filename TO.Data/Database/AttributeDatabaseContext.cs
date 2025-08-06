using Microsoft.EntityFrameworkCore;
using TO.Commons.Configs;
using Godot;
using TO.Data.DTO.GameAbilitySystem.GameplayAttribute;

namespace TO.Data.Database;

public class AttributeDatabaseContext : DbContext
{
    public DbSet<AttributeDefinitionDTO> AttributeDefinitions { get; set; }
    public DbSet<AttributeValueDTO> AttributeValues { get; set; }

    public AttributeDatabaseContext(DbContextOptions<AttributeDatabaseContext> options) : base(options) { }

    // This constructor is for use by the application, not for migrations.
    public AttributeDatabaseContext() { }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            string dbPath = ProjectSettings.GlobalizePath(ConstConfigs.AttributeDatabasePath);
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Be absolutely explicit about table names to remove any ambiguity.
        // This directly addresses the possibility that the [Table] attribute is being overlooked.
        modelBuilder.Entity<AttributeValueDTO>().ToTable("AttributeValues");
        modelBuilder.Entity<AttributeDefinitionDTO>().ToTable("Attributes"); // Map DTO to the correct "Attributes" table.

        // Configure AttributeDefinition <-> AttributeValue relationship
        modelBuilder.Entity<AttributeDefinitionDTO>()
            .HasMany<AttributeValueDTO>()
            .WithOne(v => v.AttributeDefinition)
            .HasForeignKey(v => v.AttributeType);
    }
}
