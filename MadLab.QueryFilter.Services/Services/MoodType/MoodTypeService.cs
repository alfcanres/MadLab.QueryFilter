using MadLab.QueryFilter.Domain;
using MadLab.QueryFilter.Domain.Repository;
using MadLab.QueryFilter.Services.Dto.MoodType;
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
        public MoodTypeService(IRepository<MoodType> moodTypeRepository)
        {
            _moodTypeRepository = moodTypeRepository;
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

        public async Task<IEnumerable<MoodTypeListDTO>> GetAllPaged(int currentPage, int pageSize)
        {


            //If you are thinking about performance, you can use AsNoTracking() to improve performance when you don't need to track changes to the entities.
            //I'm not using it here to keep the code simple, but you can add it if you want.
            //Also notice there is code duplication when selecting the MoodTypeListDTO, but I wanted to keep the code simple and focused on the core functionality of managing mood types.
            //However, you can create a private method to handle the mapping if you want to avoid code duplication, or even use a library like AutoMapper to handle the mapping for you.


            int skipCount = (currentPage - 1) * pageSize;


            var pagedMoodTypes = await _moodTypeRepository.Query()
                .OrderBy(m => m.Id)
                .Skip(skipCount)
                .Take(pageSize)
                .Select(m => new MoodTypeListDTO
                {
                    Id = m.Id,
                    Mood = m.Mood,
                    IsAvailable = m.IsAvailable
                }).ToListAsync();


            return pagedMoodTypes;
        }

        public async Task<IEnumerable<MoodTypeListDTO>> GetAvailableOnly()
        {
            
            var availableMoodTypes = await _moodTypeRepository.Query()
                .Where(m => m.IsAvailable)
                .Select(m => new MoodTypeListDTO
                {
                    Id = m.Id,
                    Mood = m.Mood,
                    IsAvailable = m.IsAvailable
                }).ToListAsync();


            return availableMoodTypes;

        }


    }
}
