using Microsoft.EntityFrameworkCore;
using TO.Commons.Configs;
using Godot;
using TO.Data.Converters;
using TO.Data.DTO.GameAbilitySystem.GameplayEffect;

namespace TO.Data.Database;

public class GameplayEffectDatabaseContext : DbContext
{
    public DbSet<AttributeEffectDTO> AttributeEffects { get; set; }
    public DbSet<AttributeModifierDTO> AttributeModifiers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var dbPath = ProjectSettings.GlobalizePath(ConstConfigs.GameplayEffectDatabasePath);
        optionsBuilder.UseSqlite($"Data Source={dbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AttributeEffectDTO>().ToTable("AttributeEffects");
        modelBuilder.Entity<AttributeModifierDTO>().ToTable("AttributeModifiers");

        modelBuilder.Entity<AttributeModifierDTO>().Property(e => e.AttributeType)
            .HasConversion(new AttributeDefinitionConverter());

        modelBuilder.Entity<AttributeModifierDTO>().Property(e => e.OperationType)
            .HasConversion(new ModifierOperationTypeConverter());

        modelBuilder.Entity<AttributeEffectDTO>().Property(e => e.EffectType)
            .HasConversion(new EffectTypeConverter());

        modelBuilder.Entity<AttributeEffectDTO>().Property(e => e.StackingType)
            .HasConversion(new StackingTypeConverter());

        modelBuilder.Entity<AttributeEffectDTO>().Property(e => e.Tags)
            .HasConversion(new TagsConverter());
        
    }
}
