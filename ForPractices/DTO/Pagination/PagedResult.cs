namespace ForPractices.DTO.Pagination
{
    public class PagedResult<T>
    {
        //public List<T> Items { get; set; } = new List<T>();
        //dotnet 9 version allows to use new() instead of new List<T>() for type inference
        public List<T> Items { get; set; } = new();
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);

    }
}
