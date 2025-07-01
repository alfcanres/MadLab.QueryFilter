namespace MadLab.QueryFilter.Services.Helpers
{
    internal class WithPaging<T> 
    {

        public WithPaging(int pageNumber = 1, int pageSize = 10)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public IQueryable<T> GetPaged(IQueryable<T> queryable)
        {
            int skip = (PageNumber - 1) * PageSize;

            return queryable.Skip((PageNumber - 1) * PageSize).Take(PageSize);
        }
    }   
}
