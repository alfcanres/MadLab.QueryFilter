

using MadLab.QueryFilter.Domain;
using MadLab.QueryFilter.Services.Helpers;

namespace MadLab.QueryFilter.Services.MoodTypeFilters
{
    internal class FilterBySearchTerm : IQueryFilter<MoodType>
    {
        private readonly string keyword;
        public FilterBySearchTerm(string keyword)
        {
            this.keyword = keyword;
        }
        public IQueryable<MoodType> ApplyFilter(IQueryable<MoodType> queryable)
        {
            return queryable.Where(m => m.Mood.Contains(keyword, StringComparison.OrdinalIgnoreCase));
        }
    }

}
