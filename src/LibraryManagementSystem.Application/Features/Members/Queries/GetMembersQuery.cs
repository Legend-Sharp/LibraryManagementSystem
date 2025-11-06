using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LibraryManagementSystem.Application.Abstractions;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.Features.Members.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Application.Features.Members.Queries;

public sealed record GetMembersQuery(int Page = 1, int PageSize = 20, string? Search = null) : IRequest<PagedResult<MemberDto>>, ICacheableQuery
{
    public string CacheKey => $"members:p{Page}:s{PageSize}:q{Search ?? ""}";
    public TimeSpan? AbsoluteExpiration => TimeSpan.FromSeconds(30);
}

public sealed class GetMembersQueryHandler(IAppDbContext db) : IRequestHandler<GetMembersQuery, PagedResult<MemberDto>>
{
    public async Task<PagedResult<MemberDto>> Handle(GetMembersQuery r, CancellationToken ct)
    {
        var q = db.Members.AsNoTracking();

        if (!string.IsNullOrWhiteSpace(r.Search))
        {
            var s = r.Search.Trim();
            q = q.Where(m => m.Name.Contains(s) || m.Email.Contains(s));
        }

        var total = await q.CountAsync(ct);
        var items = await q
            .OrderBy(m => m.Name)
            .Skip((r.Page - 1) * r.PageSize)
            .Take(r.PageSize)
            .Select(m => new MemberDto(m.Id, m.Name, m.Email))
            .ToListAsync(ct);

        return new PagedResult<MemberDto>(items, r.Page, r.PageSize, total);
    }
}