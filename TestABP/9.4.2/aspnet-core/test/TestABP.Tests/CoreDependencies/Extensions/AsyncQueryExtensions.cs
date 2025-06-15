using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public static class AsyncQueryExtensions
{
    public static Task<List<T>> ToListAsync<T>(
        this IQueryable<T> source,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(source.ToList());
    }

    public static Task<T> FirstOrDefaultAsync<T>(this IQueryable<T> source)
    {
        return Task.FromResult(source.FirstOrDefault());
    }

    public static Task<bool> AnyAsync<T>(this IQueryable<T> source)
    {
        return Task.FromResult(source.Any());
    }
}
