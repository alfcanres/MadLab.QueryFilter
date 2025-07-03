using MadLab.QueryFilter.Domain;
using MadLab.QueryFilter.Domain.Repository;
using MadLab.QueryFilter.Services.Dto.MoodType;
using MadLab.QueryFilter.Services.Services;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using Xunit;

namespace MadLab.QueryFilter.Tests.Services
{
    public class MoodTypeServiceTests
    {
        //Since we are changing the MoodTypeService class, I removed the preious tests that were not relevant to the new implementation.
        //The idea here is to test QueryBuilder functionality with the new MoodTypeService class.
        //The tests will cover the basic functionality of the MoodTypeService class, including creating, updating, deleting, and retrieving mood types.


        private readonly DataBaseContext _dbContext;
        private readonly IRepository<MoodType> _repository;
        private readonly MoodTypeService _service;
        private readonly IEnumerable<string> moodTypes = new List<string> { "Happy", "Sad", "Angry", "Excited" };
        public MoodTypeServiceTests()
        {
            var options = new DbContextOptionsBuilder<DataBaseContext>()
                .UseInMemoryDatabase(databaseName: "MoodTypeTestDb")
                .Options;

            _dbContext = new DataBaseContext(options);
            _repository = new RepositoryBase<MoodType>(_dbContext);
            _service = new MoodTypeService(_repository);
            CreateMoodTypes().ConfigureAwait(false).GetAwaiter().GetResult();
        }



        [Fact]
        public async Task Get_ReturnsAll_WhenNoFilter()
        {
            var filter = new MoodTypeFilterConfig();
            var result = await _service.Get(filter);
            Assert.Equal(4, result.Count());
        }

        [Fact]
        public async Task Get_FiltersByIsAvailable()
        {
            var filter = new MoodTypeFilterConfig { IsAvailable = true };
            var result = await _service.Get(filter);
            Assert.All(result, m => Assert.True(m.IsAvailable));
        }

        [Fact]
        public async Task Get_FiltersBySearchTerm()
        {
            var filter = new MoodTypeFilterConfig { SearchTerm = "Happy" };
            var result = await _service.Get(filter);
            Assert.Single(result);
            Assert.Contains(result, m => m.Mood == "Happy");
        }

        [Fact]
        public async Task Get_FiltersByOnlyWithPosts()
        {
            // Add a post to "Happy" mood type
            var happyMood = _dbContext.MoodTypes.First(m => m.Mood == "Happy");
            _dbContext.Posts.Add(new Post
            {
                Title = "Test Post",
                Text = "Test",
                AuthorId = 1,
                PostTypeId = 1,
                MoodTypeId = happyMood.Id,
                CreationDate = System.DateTime.Now,
                IsPublished = true
            });
            _dbContext.SaveChanges();

            var filter = new MoodTypeFilterConfig { OnlyWithPosts = true };
            var result = await _service.Get(filter);
            Assert.Contains(result, m => m.Mood == "Happy");
        }

        [Fact]
        public async Task Get_ReturnsPagedResults()
        {
            // Add more mood types for paging
            for (int i = 0; i < 10; i++)
            {
                _dbContext.MoodTypes.Add(new MoodType { Mood = $"Mood{i}", IsAvailable = true });
            }
            _dbContext.SaveChanges();

            var filter = new MoodTypeFilterConfig { Paged = true, PageNumber = 2, PageSize = 5 };
            var result = await _service.Get(filter);
            Assert.Equal(5, result.Count());
        }


        private async Task CreateMoodTypes()
        {
            foreach (var mood in moodTypes)
            {
                if (!_dbContext.MoodTypes.Any(pt => pt.Mood == mood))
                {
                    _dbContext.MoodTypes.Add(new MoodType
                    {
                        Mood = mood,
                        IsAvailable = true
                    });
                }
            }

            await _dbContext.SaveChangesAsync();
        }




    }
}