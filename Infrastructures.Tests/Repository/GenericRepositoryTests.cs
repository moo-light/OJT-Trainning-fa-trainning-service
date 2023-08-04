using Application.Commons;
using Application.Repositories;
using AutoFixture;
using Domain.Entities;
using Domains.Test;
using FluentAssertions;
using Infrastructures.Repositories;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructures.Tests.Repository
{
    public class GenericRepositoryTests : SetupTest
    {
        private readonly IGenericRepository<User> _genericRepository;

        public GenericRepositoryTests()
        {
            _genericRepository = new GenericRepository<User>(
                _dbContext,
                _currentTimeMock.Object,
                _claimsServiceMock.Object
                );
        }

        [Fact]
        public async Task GenericRepository_GetaAllAsync_ShouldReturnCorrectData()
        {
            var mockData = _fixture.Build<User>()
                .Without(x => x.Applications)
                .Without(x => x.Syllabuses)
                .Without(x => x.Feedbacks)
                .Without(x => x.Attendances)
                .Without(x => x.DetailTrainingClassParticipate)
                .Without(x => x.SubmitQuizzes)
                .With(x => x.IsDeleted, false)
                .CreateMany(5).ToList();
            await _dbContext.Users.AddRangeAsync(mockData);

            await _dbContext.SaveChangesAsync();


            var result = await _genericRepository.GetAllAsync();

            result.Should().BeEquivalentTo(mockData);
        }


        [Fact]
        public async Task GenericRepository_GetAllAsync_ShouldReturnEmptyWhenHaveNoData()
        {

            var result = await _genericRepository.GetAllAsync();

            result.Should().BeEmpty();
        }


        [Fact]
        public async Task GenericRepository_GetByIdAsync_ShouldReturnCorrectData()
        {
            var mockData = _fixture.Build<User>().Without(u => u.Syllabuses)
                                                 .Without(u => u.Role)
                                                 .Without(u => u.DetailTrainingClassParticipate)
                                                 .Without(u => u.Applications)
                                                 .Without(u => u.Attendances)
                                                 .Without(u => u.Feedbacks)
                                                 .Without(x => x.SubmitQuizzes)
                                                 .With(x => x.IsDeleted, false)
                                                 .Create();
            await _dbContext.Users.AddRangeAsync(mockData);

            await _dbContext.SaveChangesAsync();


            var result = await _genericRepository.GetByIdAsync(mockData.Id);

            result.Should().BeEquivalentTo(mockData);
        }


        [Fact]
        public async Task GenericRepository_GetByIdAsync_ShouldReturnEmptyWhenHaveNoData()
        {

            var result = await _genericRepository.GetByIdAsync(Guid.Empty);

            result.Should().BeNull();
        }
        [Fact]
        public async Task GenericRepository_AddAsync_ShouldReturnCorrectData()
        {
            var mockData = _fixture.Build<User>().Without(u => u.Syllabuses)
                                                 .Without(u => u.Role)
                                                 .Without(u => u.DetailTrainingClassParticipate)
                                                 .Without(u => u.Applications)
                                                 .Without(u => u.Attendances)
                                                 .Without(u => u.Feedbacks)
                                                 .Without(x => x.SubmitQuizzes)
                                                 .Create();


            await _genericRepository.AddAsync(mockData);
            var result = await _dbContext.SaveChangesAsync();

            result.Should().Be(1);
        }

        [Fact]
        public async Task GenericRepository_AddRangeAsync_ShouldReturnCorrectData()
        {
            var mockData = _fixture.Build<User>().Without(u => u.Syllabuses)
                                                 .Without(u => u.Role)
                                                 .Without(u => u.DetailTrainingClassParticipate)
                                                 .Without(u => u.Applications)
                                                 .Without(u => u.Attendances)
                                                 .Without(u => u.Feedbacks)
                                                 .Without(x => x.SubmitQuizzes)
                                                 .CreateMany(2).ToList();


            await _genericRepository.AddRangeAsync(mockData);
            var result = await _dbContext.SaveChangesAsync();

            result.Should().Be(2);
        }


        [Fact]
        public async Task GenericRepository_SoftRemove_ShouldReturnCorrectData()
        {
            var mockData = _fixture.Build<User>().Without(u => u.Syllabuses)
                                                 .Without(u => u.Role)
                                                 .Without(u => u.DetailTrainingClassParticipate)
                                                 .Without(u => u.Applications)
                                                 .Without(u => u.Attendances)
                                                 .Without(u => u.Feedbacks).Without(x => x.SubmitQuizzes)
                                                 .Create();
            _dbContext.Users.Add(mockData);
            await _dbContext.SaveChangesAsync();


            _genericRepository.SoftRemove(mockData);
            var result = await _dbContext.SaveChangesAsync();

            result.Should().Be(1);
        }

        [Fact]
        public async Task GenericRepository_Update_ShouldReturnCorrectData()
        {
            var mockData = _fixture.Build<User>().Without(u => u.Syllabuses)
                                                 .Without(u => u.Role)
                                                 .Without(u => u.DetailTrainingClassParticipate)
                                                 .Without(u => u.Applications)
                                                 .Without(u => u.Attendances)
                                                 .Without(u => u.Feedbacks).Without(x => x.SubmitQuizzes).Create();
            _dbContext.Users.Add(mockData);
            await _dbContext.SaveChangesAsync();


            _genericRepository.Update(mockData);
            var result = await _dbContext.SaveChangesAsync();

            result.Should().Be(1);
        }

        [Fact]
        public async Task GenericRepository_SoftRemoveRange_ShouldReturnCorrectData()
        {
            var mockData = _fixture.Build<User>().Without(u => u.Syllabuses)
                                                 .Without(u => u.Role)
                                                 .Without(u => u.DetailTrainingClassParticipate)
                                                 .Without(u => u.Applications)
                                                 .Without(u => u.Attendances)
                                                 .Without(u => u.Feedbacks).Without(x => x.SubmitQuizzes).CreateMany(2).ToList();
            await _dbContext.Users.AddRangeAsync(mockData);
            await _dbContext.SaveChangesAsync();


            _genericRepository.SoftRemoveRange(mockData);
            var result = await _dbContext.SaveChangesAsync();

            result.Should().Be(2);
        }

        [Fact]
        public async Task GenericRepository_UpdateRange_ShouldReturnCorrectData()
        {
            var mockData = _fixture.Build<User>().Without(u => u.Syllabuses)
                                                 .Without(u => u.Role)
                                                 .Without(u => u.DetailTrainingClassParticipate)
                                                 .Without(u => u.Applications)
                                                 .Without(u => u.Attendances)
                                                 .Without(u => u.Feedbacks).Without(x => x.SubmitQuizzes).CreateMany(2).ToList();
            await _dbContext.Users.AddRangeAsync(mockData);
            await _dbContext.SaveChangesAsync();


            _genericRepository.UpdateRange(mockData);
            var result = await _dbContext.SaveChangesAsync();

            result.Should().Be(2);
        }
        [Fact]
        public async Task ToPagination_ShouldReturnCorrectValue()
        {
            // Setup
            var mockData = _fixture.Build<User>().Without(u => u.Syllabuses)
                                                 .Without(u => u.Role)
                                                 .Without(u => u.DetailTrainingClassParticipate)
                                                 .Without(u => u.Applications)
                                                 .Without(u => u.Attendances)
                                                 .Without(u => u.SubmitQuizzes)
                                                 .Without(u => u.Feedbacks).CreateMany(2).ToList();
            await _dbContext.Users.AddRangeAsync(mockData);
            await _dbContext.SaveChangesAsync();

            var data_pageindex = mockData.OrderBy(x => x.CreationDate).First();
            // Act
            var result = await _genericRepository.ToPagination();
            var result_pagecount =await  _genericRepository.ToPagination(0,1);
            var result_pageindex =await _genericRepository.ToPagination(1,1);

            // Assert
            result.Should().BeOfType<Pagination<User>>();
            result.TotalItemsCount.Should().Be(mockData.Count);
            result.Items.Should().HaveCount(result.TotalItemsCount);
            
            result_pagecount.TotalPagesCount.Should().Be(2);
            result_pagecount.Next.Should().BeTrue();

            result_pageindex.Next.Should().BeFalse();
            result_pageindex.Previous.Should().BeTrue();
            result_pageindex.Items.First().Should().BeEquivalentTo(data_pageindex);
        }
    }
}
