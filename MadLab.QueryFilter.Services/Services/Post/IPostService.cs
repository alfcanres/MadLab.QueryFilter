
namespace MadLab.QueryFilter.Services.Services
{
    public interface IPostService
    {
        Task CreatePost(PostCreateDTO createDto);
        Task UpdatePost(PostUpdateDTO updateDto);
        Task DeletePost(int id);
        Task<PostViewDTO> GetPostById(int id);
        Task<IEnumerable<PostListDTO>> GetAllPaged(int currentPage, int pageSize);

        Task<IEnumerable<PostListDTO>> GetPublishedPaged(int currentPage, int pageSize);
        Task<IEnumerable<PostListDTO>> GetPublishedPagedByAuthor(int AuthorId, int currentPage, int pageSize);



        Task<IEnumerable<PostListDTO>> SearchPublishedPaged(string keyword, int currentPage, int pageSize);
        Task<IEnumerable<PostListDTO>> SearchPublishedPagedByAuthor(string keyword, int AuthorId, int currentPage, int pageSize);




        Task<IEnumerable<PostListDTO>> GetPublishedPagedByAuthorAndMoodTypeAndPostType(int AuthorId, int moodType, int postType, int currentPage, int pageSize);

        Task<IEnumerable<PostListDTO>> GetPublishedPagedByAuthorPostType(int AuthorId, int postType, int currentPage, int pageSize);

        Task<IEnumerable<PostListDTO>> GetPublishedPagedByAuthorAndMood(int AuthorId, int moodType, int currentPage, int pageSize);



        Task<IEnumerable<PostListDTO>> SearchPublishedPagedByAuthorAndMoodTypeAndPostType(string keyword, int AuthorId, int moodType, int postType, int currentPage, int pageSize);




    }
}
