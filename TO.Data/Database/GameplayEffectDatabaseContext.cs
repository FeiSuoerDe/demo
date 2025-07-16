using Microsoft.EntityFrameworkCore;
using TO.Data.Enities.GameAbilitySystem.GameplayEffect;
using TO.Data.Models.GameAbilitySystem.Converters;
using TO.Commons.Configs;
using Godot;

namespace TO.Data.Database
{
    public class GameplayEffectDatabaseContext : DbContext
    {
        public DbSet<AttributeEffectEntity> AttributeEffects { get; set; }
        public DbSet<AttributeModifierEntity> AttributeModifiers { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var dbPath = ProjectSettings.GlobalizePath(ConstConfigs.GameplayEffectDatabasePath);
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AttributeEffectEntity>().ToTable("AttributeEffects");
            modelBuilder.Entity<AttributeModifierEntity>().ToTable("AttributeModifiers");

            modelBuilder.Entity<AttributeModifierEntity>().Property(e => e.AttributeType)
                .HasConversion(new AttributeTypeConverter());

            modelBuilder.Entity<AttributeEffectEntity>().Property(e => e.EffectType)
                .HasConversion(new EffectTypeConverter());

            modelBuilder.Entity<AttributeEffectEntity>().Property(e => e.StackingType)
                .HasConversion(new StackingTypeConverter());

            modelBuilder.Entity<AttributeEffectEntity>().Property(e => e.Tags)
                .HasConversion(new TagsConverter());
        }
    }
}