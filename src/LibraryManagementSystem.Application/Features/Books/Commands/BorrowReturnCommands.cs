using FluentValidation;
using LibraryManagementSystem.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Application.Features.Books.Commands;

public sealed record BorrowBookCommand(Guid BookId) : IRequest;
public sealed record ReturnBookCommand(Guid BookId) : IRequest;

public sealed class BorrowBookValidator : AbstractValidator<BorrowBookCommand>
{
    public BorrowBookValidator() => RuleFor(x => x.BookId).NotEmpty();
}
public sealed class ReturnBookValidator : AbstractValidator<ReturnBookCommand>
{
    public ReturnBookValidator() => RuleFor(x => x.BookId).NotEmpty();
}

public sealed class BorrowBookHandler(IAppDbContext db) : IRequestHandler<BorrowBookCommand>
{
    public async Task<Unit> Handle(BorrowBookCommand request, CancellationToken ct)
    {
        var book = await db.Books.FirstOrDefaultAsync(b => b.Id == request.BookId, ct)
                   ?? throw new KeyNotFoundException("Book not found.");
        
        book.Borrow();
        
        await db.SaveChangesAsync(ct);
        
        return Unit.Value;
    }
}

public sealed class ReturnBookHandler(IAppDbContext db) : IRequestHandler<ReturnBookCommand>
{
    public async Task<Unit> Handle(ReturnBookCommand request, CancellationToken ct)
    {
        var book = await db.Books.FirstOrDefaultAsync(b => b.Id == request.BookId, ct)
                   ?? throw new KeyNotFoundException("Book not found.");
        
        book.Return();
        
        await db.SaveChangesAsync(ct);
        
        return Unit.Value;
    }
}