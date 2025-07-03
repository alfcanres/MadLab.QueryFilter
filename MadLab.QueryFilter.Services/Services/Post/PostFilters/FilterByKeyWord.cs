using MadLab.QueryFilter.Domain;
using MadLab.QueryFilter.Services.Helpers;

namespace MadLab.QueryFilter.Services.Services.PostFilters
{
    public class FilterByKeyWord : IQueryFilter<Post>
    {
        private readonly string keyword;
        private readonly bool filterAlsoByText = false;
        public FilterByKeyWord(string keyword, bool filterAlsoByText = false)
        {
            this.keyword = keyword;
            this.filterAlsoByText = filterAlsoByText;

        }
        public IQueryable<Post> ApplyFilter(IQueryable<Post> queryable)
        {
            if (filterAlsoByText)
            {
                return queryable.Where(post => post.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase) || post.Text.Contains(keyword, StringComparison.OrdinalIgnoreCase));
            }
            else 
            { 
                return queryable.Where(post => post.Title.Contains(keyword, StringComparison.OrdinalIgnoreCase)); 
            }
        }
    }

}
