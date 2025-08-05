using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Xunit;
using TO.Commons.Attributes;
using TO.Data; // Assume DbContext
using TO.Data.Models.GameAbilitySystem;
using TO.Data.Models.GameAbilitySystem.GameplayAttribute;
using TO.Services.Core.GameAbilitySystem;

namespace TO.UniTest.Core.GameAbilitySystem
{
    public class GameAbilitySystemIntegrationTests : IDisposable
    {
        private readonly DbContext _context;
        private readonly GameAbilityDatabaseService _databaseService;
        private readonly string _dbPath;

        public GameAbilitySystemIntegrationTests()
        {
            // Create a temporary database for testing
            _dbPath = Path.Combine(Path.GetTempPath(), $"test_attributes_{Guid.NewGuid()}.db");
            
            // Setup the database context with SQLite
            var options = new DbContextOptionsBuilder<DbContext>()
                .UseSqlite($"Data Source={_dbPath}")
                .Options;
            
            _context = new DbContext(options);
            _databaseService = new GameAbilityDatabaseService(_context);
            
            // Initialize test database with sample data
            InitializeTestDatabase();
        }

        private void InitializeTestDatabase()
        {
            // Create tables and insert test data
            _context.Database.ExecuteSqlRaw(@"
                CREATE TABLE Attributes (
                    Id INTEGER PRIMARY KEY,
                    Name TEXT NOT NULL,
                    Description TEXT
                );
                
                INSERT INTO Attributes (Id, Name, Description)
                VALUES (1, 'Health', 'Character health points');
                
                CREATE TABLE AttributeSets (
                    SetId TEXT PRIMARY KEY,
                    Name TEXT NOT NULL
                );
                
                INSERT INTO AttributeSets (SetId, Name)
                VALUES ('11111111-1111-1111-1111-111111111111', 'TestSet');
                
                CREATE TABLE AttributeValues (
                    Id INTEGER PRIMARY KEY,
                    SetId TEXT NOT NULL,
                    AttributeType TEXT NOT NULL,
                    BaseValue REAL NOT NULL,
                    MinValue REAL NOT NULL,
                    MaxValue REAL NOT NULL,
                    FOREIGN KEY (SetId) REFERENCES AttributeSets(SetId)
                );
                
                INSERT INTO AttributeValues (Id, SetId, AttributeType, BaseValue, MinValue, MaxValue)
                VALUES (1, '11111111-1111-1111-1111-111111111111', 'Character.Health', 100.0, 0.0, 200.0);

                CREATE TABLE AttributeValues_WeaponDamage (
                    Id INTEGER PRIMARY KEY,
                    SetId TEXT NOT NULL,
                    AttributeType TEXT NOT NULL,
                    BaseValue REAL NOT NULL,
                    MinValue REAL NOT NULL,
                    MaxValue REAL NOT NULL,
                    FOREIGN KEY (SetId) REFERENCES AttributeSets(SetId)
                );

                INSERT INTO AttributeValues_WeaponDamage (Id, SetId, AttributeType, BaseValue, MinValue, MaxValue)
                VALUES (1, '11111111-1111-1111-1111-111111111111', 'Ship.Weapon.Damage', 50.0, 0.0, 1000.0);
            ");
        }

        [Fact]
        public void EndToEnd_LoadAttributes_ReturnsValidData()
        {
            // Act
            var attributes = _databaseService.LoadAttributes();

            // Assert
            Assert.NotNull(attributes);
            Assert.Single(attributes);
            Assert.Equal(1, attributes.Id);
            Assert.Equal("Health", attributes.Name);
            Assert.Equal("Character health points", attributes.Description);
        }

        [Fact]
        public void EndToEnd_LoadAttributeSet_ReturnsValidData()
        {
            // Arrange
            var setId = Guid.Parse("11111111-1111-1111-1111-111111111111");

            // Act
            var attributeSet = _databaseService.LoadAttributeSet(setId);

            // Assert
            Assert.NotNull(attributeSet);
            Assert.Equal(setId, attributeSet.SetId);
            Assert.Single(attributeSet.AttributeValues);
            Assert.Equal(GameAttributes.Health, attributeSet.AttributeValues.Type);
            Assert.Equal(100.0f, attributeSet.AttributeValues.BaseValue);
            Assert.Equal(0.0f, attributeSet.AttributeValues.MinValue);
            Assert.Equal(200.0f, attributeSet.AttributeValues.MaxValue);
        }

        public void Dispose()
        {
            _context.Dispose();
            
            // Clean up the test database file
            if (File.Exists(_dbPath))
            {
                File.Delete(_dbPath);
            }
        }
    }
}
