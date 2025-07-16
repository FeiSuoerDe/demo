using System;
using System.Collections.Generic;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;
using TO.Data; // Assume DbContext
using TO.Services.Core.GameAbilitySystem;

namespace TO.UniTest.Core.GameAbilitySystem
{
    public class GameAbilityDatabaseServiceTests
    {
        private readonly Mock<DbContext> _mockContext;
        private readonly Mock<DbConnection> _mockConnection;
        private readonly Mock<DbCommand> _mockCommand;
        private readonly Mock<DbDataReader> _mockReader;
        private readonly GameAbilityDatabaseService _service;

        public GameAbilityDatabaseServiceTests()
        {
            _mockContext = new Mock<DbContext>();
            _mockConnection = new Mock<DbConnection>();
            _mockCommand = new Mock<DbCommand>();
            _mockReader = new Mock<DbDataReader>();

            _mockContext.Setup(c => c.Database.GetDbConnection()).Returns(_mockConnection.Object);
            _mockConnection.Setup(c => c.CreateCommand()).Returns(_mockCommand.Object);
            _mockCommand.Setup(c => c.ExecuteReader()).Returns(_mockReader.Object);

            _service = new GameAbilityDatabaseService(_mockContext.Object);
        }

        [Fact]
        public void LoadAttributesRaw_ReturnsData()
        {
            // Arrange
            _mockReader.SetupSequence(r => r.Read()).Returns(true).Returns(false);
            _mockReader.Setup(r => r.FieldCount).Returns(2);
            _mockReader.Setup(r => r.GetName(0)).Returns("Id");
            _mockReader.Setup(r => r.GetName(1)).Returns("Name");
            _mockReader.Setup(r => r.GetValue(0)).Returns(1);
            _mockReader.Setup(r => r.GetValue(1)).Returns("TestAttr");

            // Act
            var result = _service.LoadAttributesRaw();

            // Assert
            Assert.Single(result);
            Assert.Equal(1, result[0]["Id"]);
            Assert.Equal("TestAttr", result[0]["Name"]);
        }

        // Add more tests for other methods, error handling, etc.
    }
}