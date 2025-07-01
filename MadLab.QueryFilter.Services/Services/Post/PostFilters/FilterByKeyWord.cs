using MadLab.QueryFilter.Domain;
using MadLab.QueryFilter.Services.Helpers;

namespace MadLab.QueryFilter.Services.Services.PostFilters
{
    internal class FilterByKeyWord : IQueryFilter<Post>
    {
        private readonly string keyword;
        public FilterByKeyWord(string keyword)
        {
            this.keyword = keyword;
        }
        public IQueryable<Post> ApplyFilter(IQueryable<Post> queryable)
        {
            return queryable.Where(post => post.Title.Contains(keyword));
        }
    }

}
