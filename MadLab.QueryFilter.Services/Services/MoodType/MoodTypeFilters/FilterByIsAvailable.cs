using MadLab.QueryFilter.Domain;
using MadLab.QueryFilter.Services.Helpers;


namespace MadLab.QueryFilter.Services.MoodTypeFilters
{
    internal class FilterByIsAvailable : IQueryFilter<MoodType>
    {
        private readonly bool isAvailable;

        public FilterByIsAvailable(bool isAvailable)
        {
            this.isAvailable = isAvailable;
        }

        public IQueryable<MoodType> ApplyFilter(IQueryable<MoodType> queryable)
        {
            return queryable.Where(m => m.IsAvailable == isAvailable);
        }


    }
}
