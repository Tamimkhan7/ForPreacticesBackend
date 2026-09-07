namespace ForPractices.DTO.Pagination
{
    public class PaginationParams
    {
        private const int MaxPageSize = 50;
        private int _pageSize = 12;

        public int PageNumber { get; set; } = 1;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value < 1 ? 1 : (value > MaxPageSize ? MaxPageSize : value);
        }

        public string? SearchValue { get; set; }
        public decimal? minValue { get; set; }
        public decimal? maxValue { get; set; }
    }
}