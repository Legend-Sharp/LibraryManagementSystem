// src/LibraryManagementSystem.Application/Features/Books/Commands/DeleteBook/DeleteBookCommand.cs
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using LibraryManagementSystem.Application.Abstractions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Application.Features.Books.Commands;

public sealed record DeleteBookCommand(Guid Id) : IRequest;

public sealed class DeleteBookValidator : AbstractValidator<DeleteBookCommand>
{
    public DeleteBookValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
    }
}

public sealed class DeleteBookHandler(IAppDbContext db) : IRequestHandler<DeleteBookCommand>
{
    public async Task<Unit> Handle(DeleteBookCommand r, CancellationToken ct)
    {
        var book = await db.Books
                       .FirstOrDefaultAsync(b => b.Id == r.Id, ct)
                   ?? throw new KeyNotFoundException($"Book '{r.Id}' not found.");

        var hasActiveLoans = await db.Loans
            .AsNoTracking()
            .AnyAsync(l => l.BookId == r.Id && l.ReturnedAt == null, ct);

        if (hasActiveLoans)
            throw new InvalidOperationException("Book has active loans and cannot be deleted.");

        book.Delete();
        await db.SaveChangesAsync(ct);

        return Unit.Value;
    }
}