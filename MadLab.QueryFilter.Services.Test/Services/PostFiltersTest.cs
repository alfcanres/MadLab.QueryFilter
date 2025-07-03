using MadLab.QueryFilter.Domain;
using MadLab.QueryFilter.Services.Services.PostFilters;


namespace MadLab.QueryFilter.Services.Test.Services
{
    public class PostFiltersTest
    {
        private List<Post> GetSamplePosts()
        {
            return new List<Post>
            {
                new Post { Id = 1, Title = "Hello World", Text = "This is a test post." },
                new Post { Id = 2, Title = "Keyword Match", Text = "Contains the keyword in the text." },
                new Post { Id = 3, Title = "Another Post", Text = "Nothing special here." },
                new Post { Id = 4, Title = "HELLO again", Text = "Case insensitive test." },
                new Post { Id = 5, Title = "No match", Text = "No keyword here." }
            };
        }

        [Fact]
        public void FilterByKeyWord_FiltersByTitle()
        {
            var posts = GetSamplePosts().AsQueryable();
            var filter = new FilterByKeyWord("hello");
            var filtered = filter.ApplyFilter(posts).ToList();

            Assert.Equal(2, filtered.Count);
            Assert.Contains(filtered, p => p.Title == "Hello World");
            Assert.Contains(filtered, p => p.Title == "HELLO again");
        }

        [Fact]
        public void FilterByKeyWord_FiltersByText_WhenEnabled()
        {
            var posts = GetSamplePosts().AsQueryable();
            var filter = new FilterByKeyWord("keyword", filterAlsoByText: true);
            var filtered = filter.ApplyFilter(posts).ToList();

            Assert.Equal(2, filtered.Count);
            Assert.Equal("Keyword Match", filtered[0].Title);
            Assert.Equal("No keyword here.", filtered[1].Text);
        }

        [Fact]
        public void FilterByKeyWord_DoesNotFilterByText_WhenDisabled()
        {
            var posts = GetSamplePosts().AsQueryable();
            var filter = new FilterByKeyWord("This", filterAlsoByText: false);
            var filtered = filter.ApplyFilter(posts).ToList();

            Assert.Empty(filtered);
        }

        [Fact]
        public void FilterByKeyWord_ReturnsEmpty_WhenNoMatch()
        {
            var posts = GetSamplePosts().AsQueryable();
            var filter = new FilterByKeyWord("notfound", filterAlsoByText: true);
            var filtered = filter.ApplyFilter(posts).ToList();

            Assert.Empty(filtered);
        }

        [Fact]
        public void FilterByKeyWord_NullOrEmptyKeyword_ReturnsAll()
        {
            var posts = GetSamplePosts().AsQueryable();
            var filter = new FilterByKeyWord("");
            var filtered = filter.ApplyFilter(posts).ToList();

            Assert.Equal(posts.Count(), filtered.Count);
        }
    }
}