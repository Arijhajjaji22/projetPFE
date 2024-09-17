
using App_plateforme_de_recurtement.Models;
using App_plateforme_de_recurtement.Repositories;
using App_plateforme_de_recurtement.Services;
using Xunit;
using System.Collections.Generic;
using System.Linq;
using Moq;
using App_plateforme_de_recrutement.Tests.Repository;

namespace App_plateforme_de_recrutement.Tests.Services
{
    public class StageOfferServiceTests
    {
        private readonly FakeStageOfferRepository _fakeRepository;
        private readonly Mock<AdminStageOfferService> _mockAdminService;
        private readonly StageOfferService _service;

        public StageOfferServiceTests()
        {
            // Création de la fausse implémentation pour StageOfferRepository
            _fakeRepository = new FakeStageOfferRepository();

            // Création du mock pour AdminStageOfferService
            _mockAdminService = new Mock<AdminStageOfferService>();

            // Création du service en passant les faux objets
            _service = new StageOfferService(_fakeRepository, _mockAdminService.Object);
        }



 

        [Fact]
        public void GetOffers_ShouldReturnAllOffers()
        {
            // Arrange
            var offers = new List<StageOffer> { new StageOffer { Id = 1 }, new StageOffer { Id = 2 } };
            foreach (var offer in offers)
            {
                _fakeRepository.AddOffer(offer);
            }

            // Act
            var result = _service.GetOffers();

            // Assert
            Assert.Equal(2, result.Count());
            Assert.Contains(offers[0], result);
        }
    }
}