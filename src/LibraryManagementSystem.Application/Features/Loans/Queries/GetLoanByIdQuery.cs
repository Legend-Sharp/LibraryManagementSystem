using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LibraryManagementSystem.Application.Abstractions;
using LibraryManagementSystem.Application.Features.Loans.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Application.Features.Loans.Queries;

public sealed record GetLoanByIdQuery(Guid Id) : IRequest<LoanListItemDto?>;

public sealed class GetLoanByIdQueryHandler(IAppDbContext db) : IRequestHandler<GetLoanByIdQuery, LoanListItemDto?>
{
    public async Task<LoanListItemDto?> Handle(GetLoanByIdQuery r, CancellationToken ct)
    {
        var result =
            await (from l in db.Loans.AsNoTracking()
                    join m in db.Members.AsNoTracking() on l.MemberId equals m.Id
                    join b in db.Books.AsNoTracking() on l.BookId equals b.Id
                    where l.Id == r.Id
                    select new LoanListItemDto(
                        l.Id,
                        l.MemberId,
                        m.Name,
                        l.BookId,
                        b.Title,
                        l.BorrowedAt,
                        l.DueAt,
                        l.ReturnedAt,
                        l.ReturnedAt == null))
                .FirstOrDefaultAsync(ct);

        return result;
    }
}