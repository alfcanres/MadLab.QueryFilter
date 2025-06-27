using MadLab.QueryFilter.Domain;
using MadLab.QueryFilter.Domain.Repository;
using Microsoft.EntityFrameworkCore;

namespace MadLab.QueryFilter.Services.Services
{
    /// <summary>
    /// Provides functionality for managing posts, including creation, updating, deletion, and retrieval.
    /// </summary>
    /// <remarks>The <see cref="PostService"/> class offers methods to handle posts in a repository, such as creating
    /// new posts,  updating existing ones, deleting posts, and retrieving posts based on various criteria. It supports
    /// paginated  retrieval of posts, filtering by publication status, author, mood type, and post type, as well as
    /// searching  published posts by keywords.</remarks>
    public class PostService : IPostService
    {
        private readonly IRepository<Post> _postRepository;

        public PostService(IRepository<Post> postRepository)
        {
            _postRepository = postRepository;
        }

        public async Task CreatePost(PostCreateDTO createDto)
        {
            var post = new Post
            {
                AuthorId = createDto.AuthorId,
                PostTypeId = createDto.PostTypeId,
                MoodTypeId = createDto.MoodTypeId,
                Title = createDto.Title,
                Text = createDto.Text,
                IsPublished = createDto.IsPublished,
                CreationDate = DateTime.UtcNow,
                PublishDate = createDto.IsPublished ? DateTime.UtcNow : null
            };
            await _postRepository.AddAsync(post);
        }

        public async Task UpdatePost(PostUpdateDTO updateDto)
        {
            var post = await _postRepository.GetByIdAsync(updateDto.Id);
            if (post == null) return;

            post.AuthorId = updateDto.AuthorId;
            post.PostTypeId = updateDto.PostTypeId;
            post.MoodTypeId = updateDto.MoodTypeId;
            post.Title = updateDto.Title;
            post.Text = updateDto.Text;
            post.IsPublished = updateDto.IsPublished;
            if (updateDto.IsPublished && post.PublishDate == null)
                post.PublishDate = DateTime.UtcNow;

            await _postRepository.UpdateAsync(post);
        }

        public async Task DeletePost(int id)
        {
            await _postRepository.DeleteAsync(id);
        }

        public async Task<PostViewDTO> GetPostById(int id)
        {
            var post = await _postRepository.Query()
                .Include(p => p.Author)
                .Include(p => p.PostType)
                .Include(p => p.MoodType)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (post == null) return null;

            return new PostViewDTO
            {
                Id = post.Id,
                Title = post.Title,
                Text = post.Text,
                Author = post.Author?.UserName,
                PostTypeName = post.PostType?.Description,
                MoodTypeName = post.MoodType?.Mood,
                CreationDate = post.CreationDate,
                PublishDate = post.PublishDate,
                IsPublished = post.IsPublished,
                VoteCount = post.Votes?.Count ?? 0,
                CommentCount = post.Comments?.Count ?? 0
            };
        }

        //**************************************************************************************************************************
        //                                          NOTES
        //***************************************************************************************************************************
        //
        // This is the part I don't like, as you can see , there are many methods that are similar, but they have different parameters.
        // I would like to have a single method that can handle all these cases, but without making the code too complex or hard to read.
        // for example, I could use a combination of optional parameters, method overloading, or a more complex query with dynamic filters.
        // But I want to keep it simple and straightforward, so I will stick with the current approach for now.
        // Just wanted to show how this is getting a little messy although it works fine.
        // Now let's assume we also want to have Top 5 posts by author, mood type, post type, etc. or even 
        // a top 5 depending on mood type, or post type , or author, or a combination of these, would that imply to create even more methods?
        // See that is what I don't like, it is getting messy and hard to maintain. 
        // 
        // General idea for this Mad Lab is to fix that. And for it I will create a feature branch called feat/query-filter where
        // I will try to implement a more elegant solution that can handle all these cases with less code duplication and complexity.


        public async Task<IEnumerable<PostListDTO>> GetAllPaged(int currentPage, int pageSize)
        {
            int skip = (currentPage - 1) * pageSize;
            var posts = await _postRepository.Query()
                .Include(p => p.Author)
                .Include(p => p.PostType)
                .Include(p => p.MoodType)
                .OrderByDescending(p => p.CreationDate)
                .Skip(skip)
                .Take(pageSize)
                .Select(post => new PostListDTO
                {
                    Id = post.Id,
                    Title = post.Title,
                    Author = post.Author.UserName,
                    PostTypeName = post.PostType.Description,
                    MoodTypeName = post.MoodType.Mood,
                    CreationDate = post.CreationDate,
                    IsPublished = post.IsPublished
                })
                .ToListAsync();

            return posts;
        }

        public async Task<IEnumerable<PostListDTO>> GetPublishedPaged(int currentPage, int pageSize)
        {
            int skip = (currentPage - 1) * pageSize;
            var posts = await _postRepository.Query()
                .Include(p => p.Author)
                .Include(p => p.PostType)
                .Include(p => p.MoodType)
                .Where(p => p.IsPublished)
                .OrderByDescending(p => p.CreationDate)
                .Skip(skip)
                .Take(pageSize)
                .Select(post => new PostListDTO
                {
                    Id = post.Id,
                    Title = post.Title,
                    Author = post.Author.UserName,
                    AuthorId = post.AuthorId,
                    PostTypeName = post.PostType.Description,
                    MoodTypeId = post.MoodTypeId,
                    MoodTypeName = post.MoodType.Mood,
                    PostTypeId = post.PostTypeId,
                    CreationDate = post.CreationDate,
                    IsPublished = post.IsPublished
                })
                .ToListAsync();

            return posts;
        }

        public async Task<IEnumerable<PostListDTO>> GetPublishedPagedByAuthor(int authorId, int currentPage, int pageSize)
        {
            int skip = (currentPage - 1) * pageSize;
            var posts = await _postRepository.Query()
                .Include(p => p.Author)
                .Include(p => p.PostType)
                .Include(p => p.MoodType)
                .Where(p => p.IsPublished && p.AuthorId == authorId)
                .OrderByDescending(p => p.CreationDate)
                .Skip(skip)
                .Take(pageSize)
                .Select(post => new PostListDTO
                {
                    Id = post.Id,
                    Title = post.Title,
                    Author = post.Author.UserName,
                    AuthorId = post.AuthorId,
                    PostTypeName = post.PostType.Description,
                    MoodTypeId = post.MoodTypeId,
                    MoodTypeName = post.MoodType.Mood,
                    PostTypeId = post.PostTypeId,
                    CreationDate = post.CreationDate,
                    IsPublished = post.IsPublished
                })
                .ToListAsync();

            return posts;
        }

        public async Task<IEnumerable<PostListDTO>> SearchPublishedPaged(string keyword, int currentPage, int pageSize)
        {
            int skip = (currentPage - 1) * pageSize;
            var posts = await _postRepository.Query()
                .Include(p => p.Author)
                .Include(p => p.PostType)
                .Include(p => p.MoodType)
                .Where(p => p.IsPublished && p.Title.Contains(keyword))
                .OrderByDescending(p => p.CreationDate)
                .Skip(skip)
                .Take(pageSize)
                .Select(post => new PostListDTO
                {
                    Id = post.Id,
                    Title = post.Title,
                    Author = post.Author.UserName,
                    AuthorId = post.AuthorId,
                    PostTypeName = post.PostType.Description,
                    MoodTypeId = post.MoodTypeId,
                    MoodTypeName = post.MoodType.Mood,
                    PostTypeId = post.PostTypeId,
                    CreationDate = post.CreationDate,
                    IsPublished = post.IsPublished
                })
                .ToListAsync();

            return posts;
        }

        public async Task<IEnumerable<PostListDTO>> SearchPublishedPagedByAuthor(string keyword, int authorId, int currentPage, int pageSize)
        {
            int skip = (currentPage - 1) * pageSize;
            var posts = await _postRepository.Query()
                .Include(p => p.Author)
                .Include(p => p.PostType)
                .Include(p => p.MoodType)
                .Where(p => p.IsPublished && p.AuthorId == authorId &&
                            (p.Title.Contains(keyword)))
                .OrderByDescending(p => p.CreationDate)
                .Skip(skip)
                .Take(pageSize)
                .Select(post => new PostListDTO
                {
                    Id = post.Id,
                    Title = post.Title,
                    Author = post.Author.UserName,
                    AuthorId = post.AuthorId,
                    PostTypeName = post.PostType.Description,
                    MoodTypeId = post.MoodTypeId,
                    MoodTypeName = post.MoodType.Mood,
                    PostTypeId = post.PostTypeId,
                    CreationDate = post.CreationDate,
                    IsPublished = post.IsPublished
                })
                .ToListAsync();

            return posts;
        }

        public async Task<IEnumerable<PostListDTO>> GetPublishedPagedByAuthorAndMoodTypeAndPostType(int authorId, int moodType, int postType, int currentPage, int pageSize)
        {
            int skip = (currentPage - 1) * pageSize;
            var posts = await _postRepository.Query()
                .Include(p => p.Author)
                .Include(p => p.PostType)
                .Include(p => p.MoodType)
                .Where(p => p.IsPublished && p.AuthorId == authorId && p.MoodTypeId == moodType && p.PostTypeId == postType)
                .OrderByDescending(p => p.CreationDate)
                .Skip(skip)
                .Take(pageSize)
                .Select(post => new PostListDTO
                {
                    Id = post.Id,
                    Title = post.Title,
                    Author = post.Author.UserName,
                    AuthorId = post.AuthorId,
                    PostTypeName = post.PostType.Description,
                    MoodTypeId = post.MoodTypeId,
                    MoodTypeName = post.MoodType.Mood,
                    PostTypeId = post.PostTypeId,
                    CreationDate = post.CreationDate,
                    IsPublished = post.IsPublished
                })
                .ToListAsync();

            return posts;
        }

        public async Task<IEnumerable<PostListDTO>> GetPublishedPagedByAuthorPostType(int authorId, int postType, int currentPage, int pageSize)
        {
            int skip = (currentPage - 1) * pageSize;
            var posts = await _postRepository.Query()
                .Include(p => p.Author)
                .Include(p => p.PostType)
                .Include(p => p.MoodType)
                .Where(p => p.IsPublished && p.AuthorId == authorId && p.PostTypeId == postType)
                .OrderByDescending(p => p.CreationDate)
                .Skip(skip)
                .Take(pageSize)
                .Select(post => new PostListDTO
                {
                    Id = post.Id,
                    Title = post.Title,
                    Author = post.Author.UserName,
                    AuthorId = post.AuthorId,
                    PostTypeName = post.PostType.Description,
                    MoodTypeId = post.MoodTypeId,
                    MoodTypeName = post.MoodType.Mood,
                    PostTypeId = post.PostTypeId,
                    CreationDate = post.CreationDate,
                    IsPublished = post.IsPublished
                })
                .ToListAsync();

            return posts;
        }

        public async Task<IEnumerable<PostListDTO>> GetPublishedPagedByAuthorAndMood(int authorId, int moodType, int currentPage, int pageSize)
        {
            int skip = (currentPage - 1) * pageSize;
            var posts = await _postRepository.Query()
                .Include(p => p.Author)
                .Include(p => p.PostType)
                .Include(p => p.MoodType)
                .Where(p => p.IsPublished && p.AuthorId == authorId && p.MoodTypeId == moodType)
                .OrderByDescending(p => p.CreationDate)
                .Skip(skip)
                .Take(pageSize)
                .Select(post => new PostListDTO
                {
                    Id = post.Id,
                    Title = post.Title,
                    Author = post.Author.UserName,
                    AuthorId = post.AuthorId,
                    PostTypeName = post.PostType.Description,
                    MoodTypeId = post.MoodTypeId,
                    MoodTypeName = post.MoodType.Mood,
                    PostTypeId = post.PostTypeId,
                    CreationDate = post.CreationDate,
                    IsPublished = post.IsPublished
                })
                .ToListAsync();

            return posts;
        }

        public async Task<IEnumerable<PostListDTO>> SearchPublishedPagedByAuthorAndMoodTypeAndPostType(string keyword, int authorId, int moodType, int postType, int currentPage, int pageSize)
        {
            int skip = (currentPage - 1) * pageSize;
            var posts = await _postRepository.Query()
                .Include(p => p.Author)
                .Include(p => p.PostType)
                .Include(p => p.MoodType)
                .Where(p => p.IsPublished && p.AuthorId == authorId && p.MoodTypeId == moodType && p.PostTypeId == postType &&
                            (p.Title.Contains(keyword)))
                .OrderByDescending(p => p.CreationDate)
                .Skip(skip)
                .Take(pageSize)
                .Select(post => new PostListDTO
                {
                    Id = post.Id,
                    Title = post.Title,
                    Author = post.Author.UserName,
                    AuthorId = post.AuthorId,
                    PostTypeName = post.PostType.Description,
                    MoodTypeId = post.MoodTypeId,
                    MoodTypeName = post.MoodType.Mood,
                    PostTypeId = post.PostTypeId,
                    CreationDate = post.CreationDate,
                    IsPublished = post.IsPublished
                })
                .ToListAsync();

            return posts;
        }

    }
}