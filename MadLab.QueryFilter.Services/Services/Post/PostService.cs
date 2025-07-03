using MadLab.QueryFilter.Domain;
using MadLab.QueryFilter.Domain.Repository;
using MadLab.QueryFilter.Services.Helpers;
using MadLab.QueryFilter.Services.Services.PostFilters;
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

        //For start, wi will use the QueryBuilder to build queries for posts.
        //This will allow us to add filters, paging, and other query options in a more elegant way.
        //and what is most important, it will allow us to reuse the same query logic for different scenarios.
        //making it easier to maintain and extend the code in the future.
        private readonly QueryBuilder<Post> _queryBuilder;

        public PostService(IRepository<Post> postRepository)
        {
            _postRepository = postRepository;


            //Initialize the QueryBuilder with the post repository query, including necessary includes and no tracking.
            _queryBuilder =
                new QueryBuilder<Post>(
                    _postRepository.Query()
                        .Include(p => p.Author)
                        .Include(p => p.PostType)
                        .Include(p => p.MoodType)
                        .AsNoTracking());
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
        // So here is my approach to make the code more readable and maintainable, first, I created a method to convert the entity list to DTO list,
        // I am sure you saw that comming, but I wanted to make it clear that this is a good practice to avoid repeating the same code over and over again.
        // Then I created the QueryBuilder class, which is a generic class that allows us to build queries in a fluent way, adding filters, paging.
        // the idea here, is to avoid repeating the same filters and paging logic in every method, and of course to make the code more readable and maintainable.
        // 
        // QueryBuilder class uses IQueryFilter interface to apply filters to the query. Any class you create implementing this Interface will be compatible with the QueryBuilder.
        // In this way, you can create as many filters as you want, encapsulating the logic of how to filter the data. Take a look to FilterByIsPublished class, which is a simple filter that
        // filters by IsPublished property. Notice is receivig a boolean value to filter the data in the constructor, and in the ApplyFilter method, it applies the filter to the query
        // and returns the filtered query, and that's it. Check FilterByDateRange class, which is a filter that filters by a date range, it receives two DateTime values in the constructor and applies the filter to the query.
        //
        // Now, let's see how to use the QueryBuilder to build queries for posts.
        //
        //
        //***************************************************************************************************************************


        private IEnumerable<PostListDTO> ConvertToDto(IEnumerable<Post> entityList)
        {
            //I did not like of repeating Select many times, so I created this method to convert the entity list to DTO list.
           
            return entityList.Select(post => new PostListDTO
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
            });
        }

        public async Task<IEnumerable<PostListDTO>> GetAllPaged(int currentPage, int pageSize)
        {
            //Here is the first approach on how to use the QueryBuilder class to get all posts paginated.
            //This is a simple query that does not filter anything, just gets all posts and applies paging.
            _queryBuilder.AddPaging(currentPage, pageSize);


            //Build() method will execute the query and return the result.
            //it will go through all the filters and paging that were added to the query builder.

            IEnumerable<Post> result = await _queryBuilder.Build();

            //And finally we convert the result to DTOs.
            //Remember, it is always a good practice to convert entities to DTOs
            return ConvertToDto(result);
        }

        public async Task<IEnumerable<PostListDTO>> GetPublishedPaged(int currentPage, int pageSize)
        {

            //Now we are using the QueryBuilder to get only published posts paginated.
            //Notice we are adding a filter to get only published posts.
            _queryBuilder.AddFilter(new FilterByIsPublished(true));

            //And we are also adding paging to the query.
            _queryBuilder.AddPaging(currentPage, pageSize);

            //Now we build the query and get the result.
            IEnumerable<Post> result = await _queryBuilder.Build();


            //And we convert the result to DTOs.
            return ConvertToDto(result);
        }

        public async Task<IEnumerable<PostListDTO>> GetPublishedPagedByAuthor(int authorId, int currentPage, int pageSize)
        {
            //Here is another way to use the QueryBuilder to get published posts by author paginated.
            //Notice we are chaining the filters and paging in a fluent way, which makes the code more readable.
            //But if you prefer, you can also use the AddFilter method multiple times, this could also be helpfull if you want to add more filters
            //based on some conditions, for example, if you want to add a filter only if a certain parameter is not null or empty.But we will keep it simple for now.
            //and will provide an exmale of that later.
            IEnumerable<Post> result = await
                _queryBuilder
                .AddFilter(new FilterByIsPublished(true))
                .AddFilter(new FilterByAuthor(authorId))
                .AddPaging(currentPage, pageSize)
                .Build();

            return ConvertToDto(result);
        }

        public async Task<IEnumerable<PostListDTO>> SearchPublishedPaged(string keyword, int currentPage, int pageSize)
        {
            _queryBuilder
                .AddFilter(new FilterByIsPublished(true))
                .AddFilter(new FilterByKeyWord(keyword))
                .AddPaging(currentPage, pageSize);

            IEnumerable<Post> result = await _queryBuilder.Build();

            return ConvertToDto(result);
        }

        public async Task<IEnumerable<PostListDTO>> SearchPublishedPagedByAuthor(string keyword, int authorId, int currentPage, int pageSize)
        {
            _queryBuilder
                .AddFilter(new FilterByIsPublished(true))
                .AddFilter(new FilterByAuthor(authorId))
                .AddFilter(new FilterByKeyWord(keyword))
                .AddPaging(currentPage, pageSize);

            IEnumerable<Post> result = await _queryBuilder.Build();

            return ConvertToDto(result);
        }

        public async Task<IEnumerable<PostListDTO>> GetPublishedPagedByAuthorAndMoodTypeAndPostType(int authorId, int moodType, int postType, int currentPage, int pageSize)
        {
            _queryBuilder
                .AddFilter(new FilterByIsPublished(true))
                .AddFilter(new FilterByAuthor(authorId))
                .AddFilter(new FilterByMoodType(moodType))
                .AddFilter(new FilterByPostType(postType))
                .AddPaging(currentPage, pageSize);

            IEnumerable<Post> result = await _queryBuilder.Build();

            return ConvertToDto(result);
        }

        public async Task<IEnumerable<PostListDTO>> GetPublishedPagedByAuthorPostType(int authorId, int postType, int currentPage, int pageSize)
        {
            _queryBuilder
                .AddFilter(new FilterByIsPublished(true))
                .AddFilter(new FilterByAuthor(authorId))
                .AddFilter(new FilterByPostType(postType))
                .AddPaging(currentPage, pageSize);

            IEnumerable<Post> result = await _queryBuilder.Build();

            return ConvertToDto(result);
        }

        public async Task<IEnumerable<PostListDTO>> GetPublishedPagedByAuthorAndMood(int authorId, int moodType, int currentPage, int pageSize)
        {
            _queryBuilder
                .AddFilter(new FilterByIsPublished(true))
                .AddFilter(new FilterByAuthor(authorId))
                .AddFilter(new FilterByMoodType(moodType))
                .AddPaging(currentPage, pageSize);

            IEnumerable<Post> result = await _queryBuilder.Build();

            return ConvertToDto(result);
        }

        public async Task<IEnumerable<PostListDTO>> SearchPublishedPagedByAuthorAndMoodTypeAndPostType(string keyword, int authorId, int moodType, int postType, int currentPage, int pageSize)
        {
            //Now this looks clean and readable, we are using the QueryBuilder to build a query that filters by published posts,
            //don't you think?. On the next method we will see how to use a date range filter, remember filter is a class, we are not
            //limited to any particular parameter, so far we have used filters with a single parameter, but we can create filters with multiple parameters as well.
            //or even filters with an object as a parameter, that is the beauty of this approach.
            //And keep in mind, filter logic is encapsulated, so you can do whatever you want inside the filter class, as long as you always input an IQueryable<T>.
            //to the AddFilter method.
            _queryBuilder
                .AddFilter( new FilterByIsPublished(true))
                .AddFilter(new FilterByAuthor(authorId))
                .AddFilter(new FilterByMoodType(moodType))
                .AddFilter(new FilterByPostType(postType))
                .AddFilter(new FilterByKeyWord(keyword))
                .AddPaging(currentPage, pageSize);

            IEnumerable<Post> result = await _queryBuilder.Build();

            return ConvertToDto(result);
        }


        public async Task<IEnumerable<PostListDTO>> SearchPublishedPagedByDateRangeAndKeyword(DateTime startDate, DateTime endDate, string keyword, int currentPage, int pageSize)
        {
            //Now, here is where our QueryBuilder shines, notice we are using a date range filter.
            //Remember, a filter is actually a class that implements the IQueryFilter interface,
            //so we canb create as many filters as we want, encapsulating the logic of how to filter the data.
            //let's say you want to apply a conditional filter based on some parameters, you can do that easily by creating a new filter class
            //We will see more examples of that later.
            _queryBuilder
                .AddFilter(new FilterByIsPublished(true))
                .AddFilter(new FilterByDateRange(startDate, endDate))
                .AddFilter(new FilterByKeyWord(keyword))
                .AddPaging(currentPage, pageSize);

            IEnumerable<Post> result = await _queryBuilder.Build();

            // That's about it!, now, let's review some pros and cons of this approach:
            // PRO:
            // - The code is more readable and maintainable, we are not repeating the same code over and over again.
            // - We can easily add new filters and paging options without changing the existing code.
            // - We can easily reuse the same query logic for different scenarios, making it easier to maintain and extend the code in the future.
            // - We can easily test the filters and paging logic in isolation, making it easier to write unit tests.
            //
            // CON:
            // - The code is more complex, we are introducing a new class and interface to handle the query logic.
            // - We are adding a dependency on the QueryBuilder class, which could make the code harder to understand for someone who is not familiar with it.
            // - The use of "new" keyword to create new instances of filters could lead to performance issues if not used carefully, especially if the filters are complex and involve heavy computations. 
            // - Also "new" keyord is violating the Dependency Inversion Principle, which states that high-level modules should not depend on low-level modules, but both should depend on abstractions.

            // What do you think?, can find any other pros and cons I might have missed?


            return ConvertToDto(result);
        }
    }
}