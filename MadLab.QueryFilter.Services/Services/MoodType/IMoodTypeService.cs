using MadLab.QueryFilter.Services.Dto.MoodType;

namespace MadLab.QueryFilter.Services.Services
{
    public interface IMoodTypeService
    {
        Task CreateMoodType(MoodTypeCreateDTO crate);
        Task UpdateMoodType(MoodTypeEdit update);
        Task DeleteMoodType(int id);
        Task<MoodTypeDTO> GetMoodTypeById(int id);

        //We got rid of this methods, because it is not needed anymore, we can use Get instead

        //Task<IEnumerable<MoodTypeListDTO>> GetAvailableOnly();

        //Task<IEnumerable<MoodTypeListDTO>> GetAllPaged(int currentPage, int pageSize);

        Task<IEnumerable<MoodTypeListDTO>> Get(MoodTypeFilterConfig filterConfig);

    }
}
