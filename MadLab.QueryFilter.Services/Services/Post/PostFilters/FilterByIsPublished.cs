using MadLab.QueryFilter.Domain;
using MadLab.QueryFilter.Services.Helpers;

namespace MadLab.QueryFilter.Services.Services.PostFilters
{
    internal class FilterByIsPublished : IQueryFilter<Post>
    {
        private readonly bool _isPublished;
        public FilterByIsPublished(bool isPublished)
        {
            _isPublished = isPublished;
        }

        public IQueryable<Post> ApplyFilter(IQueryable<Post> queryable)
        {
            return queryable.Where(post => post.IsPublished == _isPublished);
        }
    }

}
