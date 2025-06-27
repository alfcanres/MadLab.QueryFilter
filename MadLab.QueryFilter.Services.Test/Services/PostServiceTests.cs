using MadLab.QueryFilter.Domain;
using MadLab.QueryFilter.Domain.Repository;
using MadLab.QueryFilter.Services.Services;
using Microsoft.EntityFrameworkCore;


namespace MadLab.QueryFilter.Services.Test.Services
{
    public class PostServiceTests
    {
        private readonly DataBaseContext _dbContext;
        private readonly IRepository<Post> _repository;
        private readonly PostService _service;
        public PostServiceTests()
        {
            var options = new DbContextOptionsBuilder<DataBaseContext>()
                .UseInMemoryDatabase(databaseName: "PostTestDB")
                .Options;

            _dbContext = new DataBaseContext(options);
            _repository = new RepositoryBase<Post>(_dbContext);
            _service = new PostService(_repository);

            DbInitializer.CreateDemoData(_dbContext).ConfigureAwait(false).GetAwaiter().GetResult();
        }


        [Fact]
        public async Task GetAllPaged_ReturnsPagedList()
        {

            // Arrange: Create demo data
       

            // Act: Call the method to get all posts paged
            var result = await _service.GetAllPaged(1, 25);

            // Assert: Check that the result is not null and contains the expected number of posts
            Assert.Equal(25, result.Count());

        }

        [Fact]
        public async Task GetPublishedPaged_ReturnsOnlyPublished()
        {
            // Arrange: Create demo data with some posts published and some not
     

            // Act: Call the method to get published posts
            var result = await _service.GetPublishedPaged(1, 10);

            // Assert: Check that the result is not null and contains only published posts
            Assert.NotNull(result);
            Assert.Equal(10, result.Count());
            Assert.All(result, p => Assert.True(p.IsPublished));
        }

        [Fact]
        public async Task GetPublishedPagedByAuthor_ReturnsOnlyByAuthor()
        {
            // Arrange: Create demo data and get the first author's ID and count of published posts by that author
            var authorId = _dbContext.Posts.First().AuthorId;
            var countPublishedPostByAuthor = _dbContext.Posts.Count(p => p.AuthorId == authorId && p.IsPublished);   


            // Act: Call the method to get published posts by author
            var result = await _service.GetPublishedPagedByAuthor(authorId, 1, countPublishedPostByAuthor);

            // Assert: Check that the result is not null, contains only published posts, and matches the author ID
            Assert.NotNull(result);
            Assert.All(result, p => Assert.True(p.IsPublished));
            Assert.Equal(countPublishedPostByAuthor, result.Count());
            Assert.All(result, p => Assert.Equal(authorId, _dbContext.Posts.First(post => post.Id == p.Id).AuthorId));
        }

        [Fact]
        public async Task SearchPublishedPaged_ReturnsMatchingKeyword()
        {
            // Arrange: Create demo data and get the first keyword from a post title and count of published posts containing that keyword
            var keyword = _dbContext.Posts.First().Title.Split(' ').First();
            var countPublishedPostByKeyword = _dbContext.Posts.Count(p => p.IsPublished && (p.Title.Contains(keyword)));    

            // Act: Call the method to search published posts by keyword
            var result = await _service.SearchPublishedPaged(keyword, 1, countPublishedPostByKeyword);

            // Assert: Check that the result is not null and contains posts matching the keyword in title or text
            Assert.NotNull(result);
            Assert.Equal(countPublishedPostByKeyword, result.Count());
            Assert.All(result, p => Assert.Contains(keyword, p.Title));
            Assert.All(result, p => Assert.True(p.IsPublished));
        }

        [Fact]
        public async Task SearchPublishedPagedByAuthor_ReturnsMatchingKeywordAndAuthor()
        {
            // Arrange: Create demo data and get the first post's keyword and author ID
            var post = _dbContext.Posts.First();
            var keyword = post.Title.Split(' ').First();
            var authorId = post.AuthorId;
            var countPublishedPostByAuthor = _dbContext.Posts.Count(p => p.IsPublished && p.AuthorId == authorId && (p.Title.Contains(keyword)));

            // Act: Call the method to search published posts by keyword and author
            var result = await _service.SearchPublishedPagedByAuthor(keyword, authorId, 1, countPublishedPostByAuthor);


            // Assert: Check that the result is not null, contains posts matching the keyword and author ID, and are published
            Assert.NotNull(result);
            Assert.Equal(countPublishedPostByAuthor, result.Count());
            Assert.All(result, p => Assert.True(p.IsPublished));
            Assert.All(result, p => Assert.Equal(authorId, p.AuthorId));
        }

        [Fact]
        public async Task GetPublishedPagedByAuthorAndMoodTypeAndPostType_ReturnsFiltered()
        {
            // Arrange: Create demo data and get the first post's author, mood type, and post type
            var post = _dbContext.Posts.First();
            var countPublishedPostByAuthorMoodTypeAndPostType = _dbContext.Posts.Count(p => p.IsPublished && p.AuthorId == post.AuthorId && p.MoodTypeId == post.MoodTypeId && p.PostTypeId == post.PostTypeId);    

            // Act: Call the method to get published posts by author, mood type, and post type
            var result = await _service.GetPublishedPagedByAuthorAndMoodTypeAndPostType(post.AuthorId, post.MoodTypeId, post.PostTypeId, 1, countPublishedPostByAuthorMoodTypeAndPostType);


            // Assert: Check that the result is not null, contains only published posts, and matches the author, mood type, and post type
            Assert.NotNull(result);
            Assert.Equal(countPublishedPostByAuthorMoodTypeAndPostType, result.Count());
            Assert.All(result, p => Assert.True(p.IsPublished));
            Assert.All(result, p => Assert.Equal(post.AuthorId, p.AuthorId));
            Assert.All(result, p => Assert.Equal(post.MoodTypeId, p.MoodTypeId));
            Assert.All(result, p => Assert.Equal(post.PostTypeId, p.PostTypeId));
        }

        [Fact]
        public async Task GetPublishedPagedByAuthorPostType_ReturnsFiltered()
        {
            // Arrange: Create demo data and get the first post's author and post type
            var post = _dbContext.Posts.First();
            var countPublishedPostByAuthorAndPostType = _dbContext.Posts.Count(p => p.IsPublished && p.AuthorId == post.AuthorId && p.PostTypeId == post.PostTypeId);   

            // Act: Call the method to get published posts by author and post type
            var result = await _service.GetPublishedPagedByAuthorPostType(post.AuthorId, post.PostTypeId, 1, countPublishedPostByAuthorAndPostType);


            // Assert: Check that the result is not null, contains only published posts, and matches the author and post type
            Assert.NotNull(result);
            Assert.Equal(countPublishedPostByAuthorAndPostType, result.Count());
            Assert.All(result, p => Assert.True(p.IsPublished));            
            Assert.All(result, p => Assert.Equal(post.AuthorId, p.AuthorId));
            Assert.All(result, p => Assert.Equal(post.PostTypeId, p.PostTypeId));
        }

        [Fact]
        public async Task GetPublishedPagedByAuthorAndMood_ReturnsFiltered()
        {
            // Arrange: Create demo data and get the first post's author and mood type

            var post = _dbContext.Posts.First();
            var countPublishedPostByAuthorAndMood = _dbContext.Posts.Count(p => p.IsPublished && p.AuthorId == post.AuthorId && p.MoodTypeId == post.MoodTypeId);

            // Act: Call the method to get published posts by author and mood type
            var result = await _service.GetPublishedPagedByAuthorAndMood(post.AuthorId, post.MoodTypeId, 1, countPublishedPostByAuthorAndMood);

            // Assert: Check that the result is not null, contains only published posts, and matches the author and mood type
            Assert.NotNull(result);
            Assert.Equal(countPublishedPostByAuthorAndMood, result.Count());
            Assert.All(result, p => Assert.True(p.IsPublished));
            Assert.All(result, p => Assert.Equal(post.AuthorId, p.AuthorId));
            Assert.All(result, p => Assert.Equal(post.MoodTypeId, p.MoodTypeId));
        }

        [Fact]
        public async Task SearchPublishedPagedByAuthorAndMoodTypeAndPostType_ReturnsFiltered()
        {
            // Arrange: Create demo data and get the first post's author, mood type, and post type

            var post = _dbContext.Posts.First();
            var keyword = post.Title.Split(' ').First();
            var countPublishedPostByAuthorAndMoodTypeAndPostType = _dbContext.Posts.Count(p => p.IsPublished && p.AuthorId == post.AuthorId && p.MoodTypeId == post.MoodTypeId && p.PostTypeId == post.PostTypeId && p.Title.Contains(keyword));

            // Act: Call the method to search published posts by keyword, author, mood type, and post type

            var result = await _service.SearchPublishedPagedByAuthorAndMoodTypeAndPostType(keyword, post.AuthorId, post.MoodTypeId, post.PostTypeId, 1, countPublishedPostByAuthorAndMoodTypeAndPostType);


            // Assert: Check that the result is not null, contains only published posts, and matches the author, mood type, post type, and keyword
            Assert.NotNull(result);
            Assert.Equal(countPublishedPostByAuthorAndMoodTypeAndPostType, result.Count());
            Assert.All(result, p => Assert.True(p.IsPublished));
            Assert.All(result, p => Assert.Equal(post.AuthorId, p.AuthorId));
            Assert.All(result, p => Assert.Equal(post.MoodTypeId, p.MoodTypeId));
            Assert.All(result, p => Assert.Equal(post.PostTypeId, p.PostTypeId));
            Assert.All(result, p => Assert.Contains(keyword, p.Title));
        }




    }
}
