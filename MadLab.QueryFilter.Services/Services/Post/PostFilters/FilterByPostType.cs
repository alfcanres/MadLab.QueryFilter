using MadLab.QueryFilter.Domain;
using MadLab.QueryFilter.Services.Helpers;

namespace MadLab.QueryFilter.Services.Services.PostFilters
{
    internal class FilterByPostType : IQueryFilter<Post>
    {
        private readonly int postTypeId;
        public FilterByPostType(int postTypeId)
        {
            this.postTypeId = postTypeId;
        }
        public IQueryable<Post> ApplyFilter(IQueryable<Post> queryable)
        {
            return queryable.Where(post => post.PostTypeId == postTypeId);
        }

    }
}
