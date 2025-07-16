using Microsoft.EntityFrameworkCore;
using TO.Data.Enities.GameAbilitySystem.GameplayAttribute;
using TO.Data.Models.GameAbilitySystem.Converters;
using TO.Commons.Configs;
using Godot;

namespace TO.Data.Database
{
    public class AttributeDatabaseContext : DbContext
    {
        public DbSet<AttributeSetEntity> AttributeSets { get; set; }
        public DbSet<BasicAttributeValueEntity> BasicAttributeValues { get; set; }
        public DbSet<ShipAttributeValueEntity> ShipAttributeValues { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            string dbPath = ProjectSettings.GlobalizePath(ConstConfigs.AttributeDatabasePath);
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AttributeSetEntity>().ToTable("AttributeSets");
            modelBuilder.Entity<BasicAttributeValueEntity>().ToTable("AttributeValues_BasicAttributes");
            modelBuilder.Entity<ShipAttributeValueEntity>().ToTable("AttributeValues_ShipAttributes");

            modelBuilder.Entity<BasicAttributeValueEntity>().Property(e => e.AttributeType)
                .HasConversion(new AttributeTypeConverter());
            modelBuilder.Entity<ShipAttributeValueEntity>().Property(e => e.AttributeType)
                .HasConversion(new AttributeTypeConverter());

          
        }
    }
}