using FluentValidation;
using LibraryManagementSystem.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Application.Books.Commands;

public sealed record BorrowBookCommand(Guid BookId) : IRequest;
public sealed record ReturnBookCommand(Guid BookId) : IRequest;

public sealed class BorrowReturnValidator : AbstractValidator<BorrowBookCommand>
{
    public BorrowReturnValidator() => RuleFor(x => x.BookId).NotEmpty();
}
public sealed class ReturnValidator : AbstractValidator<ReturnBookCommand>
{
    public ReturnValidator() => RuleFor(x => x.BookId).NotEmpty();
}

public sealed class BorrowBookHandler : IRequestHandler<BorrowBookCommand>
{
    private readonly IAppDbContext _db;
    public BorrowBookHandler(IAppDbContext db) => _db = db;

    public async Task<Unit> Handle(BorrowBookCommand request, CancellationToken ct)
    {
        var book = await _db.Books.FirstOrDefaultAsync(b => b.Id == request.BookId, ct)
                   ?? throw new KeyNotFoundException("Book not found.");
        book.Borrow();
        await _db.SaveChangesAsync(ct);
        return Unit.Value;
    }
}

public sealed class ReturnBookHandler : IRequestHandler<ReturnBookCommand>
{
    private readonly IAppDbContext _db;
    public ReturnBookHandler(IAppDbContext db) => _db = db;

    public async Task<Unit> Handle(ReturnBookCommand request, CancellationToken ct)
    {
        var book = await _db.Books.FirstOrDefaultAsync(b => b.Id == request.BookId, ct)
                   ?? throw new KeyNotFoundException("Book not found.");
        book.Return();
        await _db.SaveChangesAsync(ct);
        return Unit.Value;
    }
}