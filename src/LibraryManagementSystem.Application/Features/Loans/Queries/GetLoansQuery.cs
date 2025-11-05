using LibraryManagementSystem.Application.Abstractions;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.Features.Loans.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Application.Features.Loans.Queries;

public sealed record GetLoansQuery(
    int Page = 1,
    int PageSize = 20,
    Guid? MemberId = null,
    Guid? BookId = null,
    bool? Active = null) : IRequest<PagedResult<LoanListItemDto>>, ICacheableQuery
{
    public string CacheKey => $"loans:p{Page}:s{PageSize}:m{MemberId}:b{BookId}:a{Active}";
    public TimeSpan? AbsoluteExpiration => TimeSpan.FromSeconds(30);
}

public sealed class GetLoansQueryHandler(IAppDbContext db) : IRequestHandler<GetLoansQuery, PagedResult<LoanListItemDto>>
{
    public async Task<PagedResult<LoanListItemDto>> Handle(GetLoansQuery r, CancellationToken ct)
    {
        var q =
            from l in db.Loans.AsNoTracking()
            join m in db.Members.AsNoTracking() on l.MemberId equals m.Id
            join b in db.Books.AsNoTracking() on l.BookId equals b.Id
            select new { l, m.Name, b.Title };

        if (r.MemberId is { } mid) q = q.Where(x => x.l.MemberId == mid);
        if (r.BookId   is { } bid) q = q.Where(x => x.l.BookId == bid);
        if (r.Active   is { } act) q = q.Where(x => (x.l.ReturnedAt == null) == act);

        var total = await q.CountAsync(ct);

        var items = await q
            .OrderByDescending(x => x.l.BorrowedAt)
            .Skip((r.Page - 1) * r.PageSize)
            .Take(r.PageSize)
            .Select(x => 
                new LoanListItemDto(
                    x.l.Id,
                    x.l.MemberId,
                    x.Name,
                    x.l.BookId,
                    x.Title, 
                    x.l.BorrowedAt, 
                    x.l.DueAt, 
                    x.l.ReturnedAt,
                    x.l.ReturnedAt == null))
            .ToListAsync(ct);

        return new PagedResult<LoanListItemDto>(items, r.Page, r.PageSize, total);
    }
}