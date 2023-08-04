using Application.Interfaces;
using Application.Services;
using Application.ViewModels.SyllabusModels.UpdateSyllabusModels.HotFix;
using AutoFixture;
using Domain.Entities;
using Domains.Test;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Tests.Services
{
    public class UnitServiceTests:SetupTest
    {
        private readonly IUnitService _unitService;
        public UnitServiceTests()
        {
            _unitService = new UnitService(_unitOfWorkMock.Object, _mapperConfig);
        }

        [Fact]
        public async Task AddNewUnitHotFix_ReturnUnit()
        {
            var updateSyllabusModelMock = _fixture.Build<UpdateContentModel>().Without(x=>x.Lessons).Create();
            int sessionMock = 1;
            Guid syllabusIdMock= Guid.NewGuid();
            var mapperUnit = _mapperConfig.Map<Unit>(updateSyllabusModelMock);
            mapperUnit.Id = Guid.NewGuid();
            mapperUnit.Session= sessionMock;
            mapperUnit.SyllabusID= syllabusIdMock;

            _unitOfWorkMock.Setup(x => x.UnitRepository.AddAsync(mapperUnit)).Verifiable();

            var actualResult= await _unitService.AddNewUnitHotFix(updateSyllabusModelMock, sessionMock,syllabusIdMock);

            actualResult.Should().BeOfType<Unit>(); 
        }
    }
}
