using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using LibraryManagementSystem.Application.Abstractions;
using LibraryManagementSystem.Application.Features.Books.DTOs;
using LibraryManagementSystem.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Application.Features.Books.Commands;

public sealed record UpdateBookCommand(
    Guid Id,
    string Title,
    string Author,
    string Isbn,
    int TotalCopies
) : IRequest<BookDto>;

public sealed class UpdateBookValidator : AbstractValidator<UpdateBookCommand>
{
    public UpdateBookValidator()
    {
        RuleFor(x => x.Id).NotEmpty();
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Author).NotEmpty().MaximumLength(200);
        RuleFor(x => x.TotalCopies).GreaterThan(0);

        RuleFor(x => x.Isbn)
            .NotEmpty()
            .Must(IsValidIsbn)
            .WithMessage("ISBN must be 10 or 13 characters (digits only; ISBN-10 may end with X).");
    }

    private static bool IsValidIsbn(string raw)
    {
        var norm = new string(raw.Where(ch => char.IsDigit(ch) || ch is 'X' or 'x').ToArray());
        return norm.Length is 10 or 13;
    }
}

public sealed class UpdateBookHandler(IAppDbContext db) : IRequestHandler<UpdateBookCommand, BookDto>
{
    public async Task<BookDto> Handle(UpdateBookCommand r, CancellationToken ct)
    {
        var book = await db.Books.FirstOrDefaultAsync(b => b.Id == r.Id, ct)
                   ?? throw new KeyNotFoundException($"Book '{r.Id}' not found.");

        var norm = new string(r.Isbn.Where(ch => char.IsDigit(ch) || ch is 'X' or 'x').ToArray());
        var isbn = Isbn.Create(norm);

        var isbnTaken = await db.Books.AsNoTracking()
            .AnyAsync(b => b.Isbn.Value == isbn.Value && b.Id != r.Id, ct);
        if (isbnTaken)
            throw new InvalidOperationException("Another book already uses this ISBN.");

        book.Update(r.Title, r.Author, isbn, r.TotalCopies);

        await db.SaveChangesAsync(ct);

        return new BookDto(
            book.Id,
            book.Title,
            book.Author,
            book.Isbn.Value,
            book.TotalCopies,
            book.AvailableCopies,
            book.IsDeleted
        );
    }
}
