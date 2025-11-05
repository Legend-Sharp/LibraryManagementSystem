using LibraryManagementSystem.Application.Abstractions;
using LibraryManagementSystem.Application.Books.DTOs;
using LibraryManagementSystem.Application.Common;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Application.Books.Queries;

public sealed record GetBooksQuery(int Page = 1, int PageSize = 20, string? Search = null) : IRequest<PagedResult<BookDto>>, ICacheableQuery
{
    public string CacheKey => $"books:p{Page}:s{PageSize}:q{Search ?? ""}";
    public TimeSpan? AbsoluteExpiration => TimeSpan.FromSeconds(30);
}

public sealed class GetBooksQueryHandler(IAppDbContext db) : IRequestHandler<GetBooksQuery, PagedResult<BookDto>>
{
    public async Task<PagedResult<BookDto>> Handle(GetBooksQuery request, CancellationToken ct)
    {
        var q = db.Books.AsNoTracking();
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.Trim();
            q = q.Where(b => b.Title.Contains(s) || b.Author.Contains(s) || b.Isbn.Value.Contains(s));
        }

        var total = await q.CountAsync(ct);
        var items = await q
            .OrderBy(b => b.Title)
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(b => 
                new BookDto(
                    b.Id,
                    b.Title,
                    b.Author,
                    b.Isbn.Value,
                    b.TotalCopies,
                    b.AvailableCopies))
            .ToListAsync(ct);

        return new PagedResult<BookDto>(items, request.Page, request.PageSize, total);
    }
}