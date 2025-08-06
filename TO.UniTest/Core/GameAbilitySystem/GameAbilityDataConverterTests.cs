using System;
using System.Collections.Generic;
using Xunit;
using TO.Commons.Attributes;
using TO.Data.Models.GameAbilitySystem;
using TO.Data.Models.GameAbilitySystem.GameplayAttribute;

namespace TO.UniTest.Core.GameAbilitySystem
{
    public class GameAbilityDataConverterTests
    {
        [Fact]
        public void ConvertRawToAttributeSet_ValidData_ReturnsAttributeSet()
        {
            // Arrange
            var rawData = new List<Dictionary<string, object>>
            {
                new Dictionary<string, object>
                {
                    ["SetId"] = Guid.NewGuid().ToString(),
                    ["AttributeType"] = "Character.Health",
                    ["BaseValue"] = "100",
                    ["MinValue"] = "0",
                    ["MaxValue"] = "200"
                }
            };

            // Act
            var result = GameAbilityDataConverter.ConvertRawToAttributeSet(rawData);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result.AttributeValues);
            Assert.Equal(GameAttributes.Health, result.AttributeValues.Type);
            Assert.Equal(100f, result.AttributeValues.BaseValue);
            Assert.Equal(0f, result.AttributeValues.MinValue);
            Assert.Equal(200f, result.AttributeValues.MaxValue);
        }

        [Fact]
        public void ConvertRawToAttributeSet_InvalidType_ThrowsException()
        {
            // Arrange
            var rawData = new List<Dictionary<string, object>>
            {
                new Dictionary<string, object>
                {
                    ["SetId"] = Guid.NewGuid().ToString(),
                    ["AttributeType"] = "Invalid.Key",
                    ["BaseValue"] = "100",
                    ["MinValue"] = "0",
                    ["MaxValue"] = "200"
                }
            };

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => GameAbilityDataConverter.ConvertRawToAttributeSet(rawData));
        }

        [Fact]
        public void ConvertRawToAttributeSet_InvalidValueRange_ThrowsException()
        {
            // Arrange
            var rawData = new List<Dictionary<string, object>>
            {
                new Dictionary<string, object>
                {
                    ["SetId"] = Guid.NewGuid().ToString(),
                    ["AttributeType"] = "Character.Health",
                    ["BaseValue"] = "100",
                    ["MinValue"] = "200",
                    ["MaxValue"] = "0"
                }
            };

            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => GameAbilityDataConverter.ConvertRawToAttributeSet(rawData));
        }

        // Add more tests for other conversion methods
    }
}
