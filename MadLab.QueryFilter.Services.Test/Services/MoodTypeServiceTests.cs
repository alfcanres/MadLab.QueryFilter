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
        public async Task GetAllPaged_ReturnsPagedList()
        {


 

            var result = await _service.GetAllPaged(1, 2);

            Assert.Equal(2, result.Count());

        }

        [Fact]
        public async Task GetAvailableOnly_ReturnsOnlyAvailable()
        {


            // Set some mood types as unavailable
            var moodTypeToUpdate = await _dbContext.MoodTypes.FirstAsync(m => m.Mood == "Sad");
            moodTypeToUpdate.IsAvailable = false;
            await _dbContext.SaveChangesAsync();

            var result = await _service.GetAvailableOnly();
            

            Assert.Equal(3, result.Count());
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