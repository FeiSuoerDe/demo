using System;
using System.Collections.Generic;
using Xunit;
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
                    ["AttributeType"] = "Health",
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
            Assert.Equal(AttributeType.Health, result.AttributeValues[0].Type);
            Assert.Equal(100f, result.AttributeValues[0].BaseValue);
            Assert.Equal(0f, result.AttributeValues[0].MinValue);
            Assert.Equal(200f, result.AttributeValues[0].MaxValue);
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
                    ["AttributeType"] = "Invalid",
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
                    ["AttributeType"] = "Health",
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