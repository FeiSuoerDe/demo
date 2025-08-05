using Xunit;
using System.Linq;
using TO.Data.Database;
using TO.Data.DTO.GameAbilitySystem.GameplayAttribute;
using TO.Data.Registries;
using Microsoft.EntityFrameworkCore;
using TO.Commons.Configs;
using Godot;
using System.Collections.Generic;
using TO.Data.Models.GameAbilitySystem.GameplayAttribute;

namespace TO.UniTest.Core.GameAbilitySystem
{
    public class DatabaseMigrationTests
    {
        /// <summary>
        /// A one-time test to migrate data from the old database structure to the new one.
        /// 1. Make sure your old 'AttributeDatabase.db' is in the correct location.
        /// 2. Run this test once.
        /// 3. After successful execution, you can rename 'AttributeDatabase.db.new' to 'AttributeDatabase.db'.
        /// 4. Update ConstConfigs.AttributeDatabasePath to point to the new database file if you haven't already.
        /// 5. You can then remove this test.
        /// </summary>
        [Fact]
        public void MigrateDatabase_ToV2_TPH_Structure()
        {
            // Step 1: Set up DbContexts for both old and new databases
            using var oldDbContext = CreateOldDbContext();
            using var newDbContext = new AttributeDatabaseContext(); // Uses the new path from its OnConfiguring

            // Step 2: Ensure the new database is created
            newDbContext.Database.EnsureDeleted();
            newDbContext.Database.EnsureCreated();

            // Step 3: Migrate AttributeDefinitions
            var attributeDefinitions = AttributeRegistry.GetAll();
            var attributeDefinitionDTOs = attributeDefinitions.Select(ad => new AttributeDefinitionDTO
            {
                Id = ad.Id,
                Key = ad.Key,
                Category = ad.Category
            }).ToList();
            newDbContext.AttributeDefinitions.AddRange(attributeDefinitionDTOs);
            newDbContext.SaveChanges();
            var definitionsMap = attributeDefinitionDTOs.ToDictionary(dto => dto.Key, dto => dto.Id);

            // Step 4: Migrate AttributeSets and AttributeValues
            var oldAttributeSets = oldDbContext.AttributeSets.ToList();
            
            foreach (var oldSet in oldAttributeSets)
            {
                // Add the set itself
                newDbContext.AttributeSets.Add(oldSet);

                // Migrate All Attributes into the single AttributeValues table
                var allValues = new List<AttributeValueDTO>();

                allValues.AddRange(oldDbContext.BasicAttributeValues
                    .Where(v => v.AttributeSetId == oldSet.Id)
                    .Select(oldValue => new AttributeValueDTO
                    {
                        AttributeSetId = oldValue.AttributeSetId,
                        AttributeDefinitionId = definitionsMap[oldValue.AttributeType.Key],
                        BaseValue = oldValue.BaseValue,
                        MinValue = oldValue.MinValue,
                        MaxValue = oldValue.MaxValue
                    }));

                allValues.AddRange(oldDbContext.ShipAttributeValues
                    .Where(v => v.AttributeSetId == oldSet.Id)
                    .Select(oldValue => new AttributeValueDTO
                    {
                        AttributeSetId = oldValue.AttributeSetId,
                        AttributeDefinitionId = definitionsMap[oldValue.AttributeType.Key],
                        BaseValue = oldValue.BaseValue,
                        MinValue = oldValue.MinValue,
                        MaxValue = oldValue.MaxValue
                    }));

                allValues.AddRange(oldDbContext.WeaponDamageAttributeValues
                    .Where(v => v.AttributeSetId == oldSet.Id)
                    .Select(oldValue => new AttributeValueDTO
                    {
                        AttributeSetId = oldValue.AttributeSetId,
                        AttributeDefinitionId = definitionsMap[oldValue.AttributeType.Key],
                        BaseValue = oldValue.BaseValue,
                        MinValue = 0, // Assuming default MinValue
                        MaxValue = 0  // Assuming default MaxValue
                    }));
                
                newDbContext.AttributeValues.AddRange(allValues);
            }

            newDbContext.SaveChanges();

            // Optional: Verification
            Assert.Equal(oldAttributeSets.Count, newDbContext.AttributeSets.Count());
            var totalOldValues = oldDbContext.BasicAttributeValues.Count() + oldDbContext.ShipAttributeValues.Count() + oldDbContext.WeaponDamageAttributeValues.Count();
            Assert.Equal(totalOldValues, newDbContext.AttributeValues.Count());
        }

        private OldAttributeDatabaseContext CreateOldDbContext()
        {
            var options = new DbContextOptionsBuilder<OldAttributeDatabaseContext>()
                .UseSqlite($"Data Source={ProjectSettings.GlobalizePath(ConstConfigs.AttributeDatabasePath)}")
                .Options;
            return new OldAttributeDatabaseContext(options);
        }
    }

    /// <summary>
    /// A temporary DbContext class representing the OLD database structure.
    /// This is needed only for the migration process.
    /// </summary>
    public class OldAttributeDatabaseContext : DbContext
    {
        public DbSet<AttributeSetDTO> AttributeSets { get; set; }
        public DbSet<OldBasicAttributeValueDTO> BasicAttributeValues { get; set; }
        public DbSet<OldShipAttributeValueDTO> ShipAttributeValues { get; set; }
        public DbSet<OldWeaponDamageAttributeValueDTO> WeaponDamageAttributeValues { get; set; }

        public OldAttributeDatabaseContext(DbContextOptions<OldAttributeDatabaseContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AttributeSetDTO>().ToTable("AttributeSets");
            modelBuilder.Entity<OldBasicAttributeValueDTO>().ToTable("AttributeValues_BasicAttributes");
            modelBuilder.Entity<OldShipAttributeValueDTO>().ToTable("AttributeValues_ShipAttributes");
            modelBuilder.Entity<OldWeaponDamageAttributeValueDTO>().ToTable("AttributeValues_WeaponDamage");

            modelBuilder.Entity<OldBasicAttributeValueDTO>().Property(e => e.AttributeType).HasConversion(new AttributeDefinitionConverter());
            modelBuilder.Entity<OldShipAttributeValueDTO>().Property(e => e.AttributeType).HasConversion(new AttributeDefinitionConverter());
            modelBuilder.Entity<OldWeaponDamageAttributeValueDTO>().Property(e => e.AttributeType).HasConversion(new AttributeDefinitionConverter());
        }
    }

    // Temporary DTO classes representing the OLD data structure
    public class OldBasicAttributeValueDTO
    {
        public int Id { get; set; }
        public string AttributeSetId { get; set; }
        public AttributeDefinition AttributeType { get; set; }
        public float MinValue { get; set; }
        public float MaxValue { get; set; }
        public float BaseValue { get; set; }
    }

    public class OldShipAttributeValueDTO
    {
        public int Id { get; set; }
        public string AttributeSetId { get; set; }
        public AttributeDefinition AttributeType { get; set; }
        public float BaseValue { get; set; }
        public float MinValue { get; set; }
        public float MaxValue { get; set; }
    }

    public class OldWeaponDamageAttributeValueDTO
    {
        public int Id { get; set; }
        public string AttributeSetId { get; set; }
        public AttributeDefinition AttributeType { get; set; }
        public float BaseValue { get; set; }
    }
}
