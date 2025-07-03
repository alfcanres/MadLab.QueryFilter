using MadLab.QueryFilter.Domain;
using MadLab.QueryFilter.Services.Helpers;


namespace MadLab.QueryFilter.Services.Services.PostFilters
{
    internal class FilterByDateRange : IQueryFilter<Post>
    {
        private readonly DateTime _startDate;
        private readonly DateTime _endDate;
        public FilterByDateRange(DateTime startDate, DateTime endDate)
        {
            _startDate = startDate;
            _endDate = endDate;
        }
        public IQueryable<Post> ApplyFilter(IQueryable<Post> queryable)
        {
            return queryable.Where(post => post.PublishDate >= _startDate && post.PublishDate <= _endDate);
        }
    }

}
