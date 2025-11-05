using LibraryManagementSystem.Application.Common;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace LibraryManagementSystem.Application.Behaviors;

public sealed class CachingBehavior<TRequest, TResponse>(IMemoryCache cache) : IPipelineBehavior<TRequest, TResponse> where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        if (request is not ICacheableQuery cq) return await next();

        if (cache.TryGetValue(cq.CacheKey, out TResponse? cached) && cached is not null)
            return cached;

        var result = await next();
        cache.Set(cq.CacheKey, result, cq.AbsoluteExpiration ?? TimeSpan.FromMinutes(2));
        return result;
    }
}