using FluentValidation;
using LibraryManagementSystem.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Application.Loans.Commands;

public sealed record ReturnBookForMemberCommand(Guid LoanId) : IRequest;

public sealed class ReturnBookForMemberValidator : AbstractValidator<ReturnBookForMemberCommand>
{
    public ReturnBookForMemberValidator() => RuleFor(x => x.LoanId).NotEmpty();
}

public sealed class ReturnBookForMemberHandler(IAppDbContext db) : IRequestHandler<ReturnBookForMemberCommand>
{
    public async Task<Unit> Handle(ReturnBookForMemberCommand request, CancellationToken ct)
    {
        var now = DateTime.UtcNow;

        var loan = await db.Loans.FirstOrDefaultAsync(l => l.Id == request.LoanId, ct)
                   ?? throw new KeyNotFoundException("Loan not found.");

        if (!loan.IsActive) return Unit.Value;

        loan.Return(now);

        await db.SaveChangesAsync(ct);
        
        return Unit.Value;
    }

}