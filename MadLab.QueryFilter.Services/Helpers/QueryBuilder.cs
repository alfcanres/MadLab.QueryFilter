using Microsoft.EntityFrameworkCore;

namespace MadLab.QueryFilter.Services.Helpers
{
    
    internal class QueryBuilder<Entitty> where Entitty : class
    {

        private WithPaging<Entitty> _withPaging;
        private IQueryable<Entitty> _query;
        private List<IQueryFilter<Entitty>> _queryFilters = new List<IQueryFilter<Entitty>>();

        public QueryBuilder(IQueryable<Entitty> query)
        {
            _query = query ?? throw new ArgumentNullException(nameof(query), "Query cannot be null");
        }

        public QueryBuilder<Entitty> AddFilter(IQueryFilter<Entitty> queryFilter)
        {

            if (_queryFilters.Any(f => f.GetType().Name == queryFilter.GetType().Name))
            {
                throw new InvalidOperationException($"Filter {queryFilter.GetType().Name} has already been added.");
            }

            _queryFilters.Add(queryFilter);

            return this;
        }

        public async Task<IEnumerable<Entitty>> Build()
        {

            foreach (var filter in _queryFilters)
            {
                _query = filter.ApplyFilter(_query);
            }

            if (_withPaging != null)
            {
                _query = _withPaging.GetPaged(_query);
            }

            return await _query.ToListAsync();
        }

        public QueryBuilder<Entitty> AddPaging(int pageNumber, int pageSize)
        {
            if (_withPaging != null)
            {
                throw new InvalidOperationException("Paging has already been set. You cannot set it again.");
            }

            _withPaging = new WithPaging<Entitty>(pageNumber, pageSize);

            return this;
        }

    }
}
