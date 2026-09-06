using ForPractices.DTO.Pagination;
using Microsoft.EntityFrameworkCore;

namespace ForPractices.Extensions
{
    public static class QueryableExtensions
    {
        public static async Task<PagedResult<T>> ToPagedResultAsync<T>(this IQueryable<T> query, PaginationParams pagination) where T : class
        {
            var totalCount = await query.CountAsync();

            var items = await query
                .AsNoTracking()
                .Skip((pagination.PageNumber - 1) * pagination.PageSize)
                .Take(pagination.PageSize)
                .ToListAsync();

            return new PagedResult<T>
            {
                Items = items,
                PageNumber = pagination.PageNumber,
                PageSize = pagination.PageSize,
                TotalCount = totalCount
            };
        }
    }
}