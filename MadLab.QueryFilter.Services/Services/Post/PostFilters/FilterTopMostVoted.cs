using MadLab.QueryFilter.Domain;
using MadLab.QueryFilter.Services.Helpers;

namespace MadLab.QueryFilter.Services.PostFilters
{
    internal class FilterTopMostVoted : IQueryFilter<Post>
    {
        private readonly int topCount;
        public FilterTopMostVoted(int topCount)
        {
            this.topCount = topCount;
        }
        public IQueryable<Post> ApplyFilter(IQueryable<Post> queryable)
        {
            return queryable.OrderByDescending(post => post.Votes.Count >= 1).Take(topCount);
        }
    }
}
