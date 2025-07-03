using MadLab.QueryFilter.Domain;
using MadLab.QueryFilter.Services.Helpers;

namespace MadLab.QueryFilter.Services.MoodTypeFilters
{
    internal class FilterByOnlyWithPosts : IQueryFilter<MoodType>
    {
        public IQueryable<MoodType> ApplyFilter(IQueryable<MoodType> queryable)
        {
            return queryable.Where(m => m.Posts.Any());
        }
    }

}
