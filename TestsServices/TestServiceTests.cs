using Moq;
using Xunit;
using App_plateforme_de_recurtement.Models;
using App_plateforme_de_recurtement.Repositories;
using App_plateforme_de_recurtement.Services;
using System.Collections.Generic;
using App_plateforme_de_recurtement.Data.Repositories;
using App_plateforme_de_recurtement.Data;

namespace App_plateforme_de_recurtement.Tests.Services
{
    public class TestServiceTests
    {
        [Fact]
        public void GetAllTests_ShouldReturnListOfTests()
        {
            // Arrange
            var mockRepository = new Mock<TestRepository>();
            var mockContext = new Mock<ApplicationDbContext>();

            // Créez des données fictives pour les tests
            var testList = new List<Test>
            {
                new Test { Id = 1, Title = "Test 1", Description = "Description 1" },
                new Test { Id = 2, Title = "Test 2", Description = "Description 2" }
            };

            mockRepository.Setup(repo => repo.GetAllTests()).Returns(testList);

            var service = new TestService(mockRepository.Object, mockContext.Object);

            // Act
            var result = service.GetAllTests();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, t => t.Title == "Test 1");
            Assert.Contains(result, t => t.Title == "Test 2");
        }
    }
}
