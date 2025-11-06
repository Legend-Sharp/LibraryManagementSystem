using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using LibraryManagementSystem.Application.Abstractions;
using LibraryManagementSystem.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Application.Features.Loans.Commands;

public sealed record BorrowBookForMemberCommand(Guid MemberId, Guid BookId, DateTime? DueAt) : IRequest<Guid>;

public sealed class BorrowBookForMemberValidator : AbstractValidator<BorrowBookForMemberCommand>
{
    public BorrowBookForMemberValidator()
    {
        RuleFor(x => x.MemberId).NotEmpty();
        RuleFor(x => x.BookId).NotEmpty();
    }
}

public sealed class BorrowBookForMemberHandler(IAppDbContext db) : IRequestHandler<BorrowBookForMemberCommand, Guid>
{
    public async Task<Guid> Handle(BorrowBookForMemberCommand request, CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        _ = await db.Members.FirstOrDefaultAsync(m => m.Id == request.MemberId, ct)
            ?? throw new KeyNotFoundException("Member not found.");

        _ = await db.Books.AsNoTracking().FirstOrDefaultAsync(b => b.Id == request.BookId, ct)
            ?? throw new KeyNotFoundException("Book not found.");

        var hasActive = await db.Loans.AnyAsync(l => l.MemberId == request.MemberId && l.BookId == request.BookId && l.ReturnedAt == null, ct);
        if (hasActive) throw new InvalidOperationException("Member already has an active loan for this book.");

        var loan = Loan.Create(request.MemberId, request.BookId, now, request.DueAt);
        
        await db.Loans.AddAsync(loan, ct);
        await db.SaveChangesAsync(ct);

        return loan.Id;
    }
}
