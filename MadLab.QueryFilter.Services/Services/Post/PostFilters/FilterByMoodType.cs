using MadLab.QueryFilter.Domain;
using MadLab.QueryFilter.Services.Helpers;
namespace MadLab.QueryFilter.Services.Services.PostFilters
{
    internal class FilterByMoodType : IQueryFilter<Post>
    {
        private readonly int moodTypeId;
        public FilterByMoodType(int moodTypeId)
        {
            this.moodTypeId = moodTypeId;
        }
        public IQueryable<Post> ApplyFilter(IQueryable<Post> queryable)
        {
            return queryable.Where(post => post.MoodTypeId == moodTypeId);
        }
    }
}
