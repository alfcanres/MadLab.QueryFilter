namespace MadLab.QueryFilter.Services.Helpers
{
    internal interface IQueryFilter<T> where T : class
    {
        IQueryable<T> ApplyFilter(IQueryable<T> queryable);

    }
}
