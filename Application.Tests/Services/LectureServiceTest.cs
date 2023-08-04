using Application.Interfaces;
using Application.Services;
using Application.ViewModels.SyllabusModels;
using Application.ViewModels.SyllabusModels.UpdateSyllabusModels.HotFix;
using AutoFixture;
using AutoMapper;
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
    public class LectureServiceTest:SetupTest
    {
        private readonly ILectureService _lectureService;
        public LectureServiceTest()
        {
            _lectureService= new LectureService(_unitOfWorkMock.Object, _mapperConfig);
        }

        [Fact]
        public async Task AddNewLecture_ReturnLecture()
        {
            var lectureMock = _fixture.Build<Lecture>()
                                        .Without(x=>x.DetailUnitLectures)
                                        .Without(x=>x.TrainingMaterials)
                                        .Without(x=>x.Gradings)
                                        .Without(x=>x.Assignments)
                                        .Without(x=>x.AuditPlans)
                                        .Without(x=>x.Quiz)
                                        .Create();
            _unitOfWorkMock.Setup(x => x.LectureRepository.AddAsync(lectureMock)).Verifiable();
            var mapLectureMock = _mapperConfig.Map<LectureDTO>(lectureMock);

            var actualResult = await _lectureService.AddNewLecture(mapLectureMock);

            actualResult.Should().BeOfType<Lecture>();
        }

        [Fact]
        public async Task AddNewDetailLecture_ReturnDetailLecture()
        {
            var lectuerMock = _fixture.Build<Lecture>()
                                        .Without(x=>x.DetailUnitLectures)
                                        .Without(x=>x.TrainingMaterials)
                                        .Without(x=>x.Gradings)
                                        .Without(x=>x.Assignments)
                                        .Without(x=>x.AuditPlans)
                                        .Without(x=>x.Quiz)
                                        .Create();
            var unitMock = _fixture.Build<Unit>()
                                    .Without(x=>x.DetailUnitLectures)
                                    .Without(x=>x.Syllabus)
                                    .Create();
            var detailUnitLectureMock = _fixture.Build<DetailUnitLecture>()
                                                .Without(x=>x.Unit)
                                                .Without(x=>x.Lecture)
                                                .Create();
            _unitOfWorkMock.Setup(x => x.DetailUnitLectureRepository.AddAsync(detailUnitLectureMock)).Verifiable();

            var actualResult = await _lectureService.AddNewDetailLecture(lectuerMock, unitMock);

            actualResult.Should().BeOfType<DetailUnitLecture>();

        }

        [Fact]
        public async Task AddNewLectureHotFix_ReturnLecture()
        {
            var updateLessonModelMock=_fixture.Build<UpdateLessonModel>().Create();
            var mapperupdateLessonMock = _mapperConfig.Map<Lecture>(updateLessonModelMock);
            _unitOfWorkMock.Setup(x=>x.LectureRepository.AddAsync(mapperupdateLessonMock)).Verifiable();

            var actualResult =await _lectureService.AddNewLectureHotFix(updateLessonModelMock);

            actualResult.Should().BeOfType<Lecture>();
        }
    }
}
