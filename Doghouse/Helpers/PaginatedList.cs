namespace Doghouse.Helpers
{
    public class PaginatedList<T>
    {
        public List<T> Items { get; }
        public int TotalCount { get; }
        public int CurrentPage { get; }
        public int PageSize { get; }

        public PaginatedList(List<T> items, int totalCount, int currentPage, int pageSize)
        {
            Items = items;
            TotalCount = totalCount;
            CurrentPage = currentPage;
            PageSize = pageSize;
        }

        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    }
}
