using MadLab.QueryFilter.Services.Dto.MoodType;

namespace MadLab.QueryFilter.Services.Services
{
    public interface IMoodTypeService
    {
        Task CreateMoodType(MoodTypeCreateDTO crate);
        Task UpdateMoodType(MoodTypeEdit update);
        Task DeleteMoodType(int id);
        Task<MoodTypeDTO> GetMoodTypeById(int id);
        Task<IEnumerable<MoodTypeListDTO>> GetAvailableOnly();

        Task<IEnumerable<MoodTypeListDTO>> GetAllPaged(int currentPage, int pageSize);
    }
}
