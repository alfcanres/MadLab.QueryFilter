using MadLab.QueryFilter.Domain;
using MadLab.QueryFilter.Domain.Repository;
using MadLab.QueryFilter.Services.Dto.MoodType;
using MadLab.QueryFilter.Services.Helpers;
using MadLab.QueryFilter.Services.MoodTypeFilters;
using Microsoft.EntityFrameworkCore;

namespace MadLab.QueryFilter.Services.Services
{


    /// <summary>
    /// Provides functionality for managing mood types, including creating, updating, deleting,  and retrieving mood
    /// types. This service supports operations for both paginated and filtered  retrieval of mood types.
    /// This service interacts with a repository to perform CRUD operations on mood types. It is
    /// designed  to handle data transfer objects (DTOs) for input and output, ensuring separation of concerns between 
    /// the domain model and external consumers. Use this service to manage mood types in applications  where
    /// mood-related data is required.
    /// </summary>
    /// <remarks>
    /// Please note, this a pretty basic service, it lacks of advanced features such as validation, error handling, and logging.
    /// I did not include these features to keep the code simple and focused on the core functionality of managing mood types.
    ///</remarks>
    public class MoodTypeService : IMoodTypeService
    {
        private readonly IRepository<MoodType> _moodTypeRepository;
        private readonly IQueryable<MoodType> _moodTypesQuery;
        private readonly QueryBuilder<MoodType> _queryBuilder;
        public MoodTypeService(IRepository<MoodType> moodTypeRepository)
        {
            _moodTypeRepository = moodTypeRepository;

            _moodTypesQuery = _moodTypeRepository.Query()
                .Include(t => t.Posts)
                .AsNoTracking();

            _queryBuilder = new QueryBuilder<MoodType>(_moodTypesQuery);
        }

        public async Task CreateMoodType(MoodTypeCreateDTO crate)
        {
            var moodType = new MoodType
            {
                Mood = crate.Mood,
                IsAvailable = crate.IsAvailable
            };

            await _moodTypeRepository.AddAsync(moodType);

        }

        public async Task UpdateMoodType(MoodTypeEdit update)
        {
            var moodType = await _moodTypeRepository.GetByIdAsync(update.Id);
            moodType.Mood = update.Mood;
            moodType.IsAvailable = update.IsAvailable;

            await _moodTypeRepository.UpdateAsync(moodType);

        }

        public async Task DeleteMoodType(int id)
        {
            var moodType = await _moodTypeRepository.GetByIdAsync(id);
            await _moodTypeRepository.DeleteAsync(id);

        }
        public async Task<MoodTypeDTO> GetMoodTypeById(int id)
        {
            var model = await _moodTypeRepository.GetByIdAsync(id);

            return new MoodTypeDTO
            {
                Id = model?.Id ?? 0,
                Mood = model?.Mood ?? string.Empty,
                IsAvailable = model?.IsAvailable ?? false
            };
        }


        //Now remember we used to have two methods for getting mood types, one for getting all available mood types and another for getting paginated mood types.
        // We got rid of these methods, because we can use the Get method with a filter configuration to achieve the same functionality.
        // This is another approach to simplify the service and make it more flexible.
        // I don't personaly like this that much since we can end up adding to much complexity to a single method,
        // however I think it is a good exercise to show how we can use the QueryBuilder to filter and paginate the results.

        public async Task<IEnumerable<MoodTypeListDTO>> Get(MoodTypeFilterConfig filterConfig)
        {


            if(filterConfig.IsAvailable.HasValue)
            {
                _queryBuilder.AddFilter(new FilterByIsAvailable(filterConfig.IsAvailable.Value));
            }


            if (!string.IsNullOrEmpty(filterConfig.SearchTerm))
            {
                _queryBuilder.AddFilter(new FilterBySearchTerm(filterConfig.SearchTerm));
            }


            if (filterConfig.OnlyWithPosts)
            {
                _queryBuilder.AddFilter(new FilterByOnlyWithPosts());
            }


            if (filterConfig.Paged)
            {
                _queryBuilder.AddPaging(filterConfig.PageNumber, filterConfig.PageSize);
            }



            var result = await _queryBuilder.Build();

            return result.Select(m => new MoodTypeListDTO
            {
                Id = m.Id,
                Mood = m.Mood,
                IsAvailable = m.IsAvailable
            }).ToList();

        }
    }
}
