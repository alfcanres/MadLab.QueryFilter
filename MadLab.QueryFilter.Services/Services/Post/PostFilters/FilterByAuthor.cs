

using MadLab.QueryFilter.Domain;
using MadLab.QueryFilter.Services.Helpers;

namespace MadLab.QueryFilter.Services.Services.PostFilters
{
    internal class FilterByAuthor : IQueryFilter<Post>
    {
        private readonly int _authorId;
        public FilterByAuthor(int authorId)
        {
            this._authorId = authorId;
        }
        public IQueryable<Post> ApplyFilter(IQueryable<Post> queryable)
        {
            return queryable.Where(post => post.Author.Id == _authorId);
        }

    }
}
