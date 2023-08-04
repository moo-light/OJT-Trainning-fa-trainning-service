using AutoFixture;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebAPI.Controllers;

namespace Domains.Test.Controller
{
    public class UnitClassTest : SetupTest
    {
        private readonly UnitController _unitController;

        public UnitClassTest()
        {
            _unitController = new UnitController(_unitServiceMock.Object);
        }

        [Fact]
        public async Task GetAllUnitBySyllabusID_ShouldReturnUnit()
        {
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList()
                 .ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            var mockData = _fixture.Build<Unit>().Without(x => x.Syllabus).Without(x => x.DetailUnitLectures).Create<Unit>();
            _unitServiceMock.Setup(x => x.GetSyllabusDetail(mockData.Id));
            var result = await _unitController.GetUnitBSyllabusID(mockData.Id);

            Assert.NotNull(result);




        }
    }
}
