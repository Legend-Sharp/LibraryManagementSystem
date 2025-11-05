using LibraryManagementSystem.Application.Abstractions;
using LibraryManagementSystem.Application.Features.Loans.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Application.Features.Loans.Queries;

public sealed record GetActiveLoansByMemberQuery(Guid MemberId) : IRequest<IReadOnlyList<LoanDto>>;

public sealed class GetActiveLoansByMemberHandler(IAppDbContext db) : IRequestHandler<GetActiveLoansByMemberQuery, IReadOnlyList<LoanDto>>
{
    public async Task<IReadOnlyList<LoanDto>> Handle(GetActiveLoansByMemberQuery request, CancellationToken ct)
    {
        var q =
            from l in db.Loans.AsNoTracking()
            join b in db.Books.AsNoTracking() on l.BookId equals b.Id
            where l.MemberId == request.MemberId && l.ReturnedAt == null
            orderby l.BorrowedAt descending
            select new LoanDto(l.Id, l.MemberId, l.BookId, b.Title, l.BorrowedAt, l.DueAt, true);

        return await q.ToListAsync(ct);
    }
}