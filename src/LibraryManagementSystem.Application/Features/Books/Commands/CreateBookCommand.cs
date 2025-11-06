using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using LibraryManagementSystem.Application.Abstractions;
using LibraryManagementSystem.Application.Features.Books.DTOs;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.ValueObjects;
using MediatR;

namespace LibraryManagementSystem.Application.Features.Books.Commands;

public sealed record CreateBookCommand(string Title, string Author, string Isbn, int TotalCopies = 1) : IRequest<BookDto>;

public sealed class CreateBookValidator : AbstractValidator<CreateBookCommand>
{
    public CreateBookValidator()
    {
        RuleFor(x => x.Title).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Author).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Isbn).NotEmpty();
        RuleFor(x => x.TotalCopies).GreaterThan(0);
    }
}

public sealed class CreateBookHandler(IAppDbContext db) : IRequestHandler<CreateBookCommand, BookDto>
{
    public async Task<BookDto> Handle(CreateBookCommand request, CancellationToken ct)
    {
        var book = Book.Create(request.Title, request.Author, Isbn.Create(request.Isbn), request.TotalCopies);
        
        await db.Books.AddAsync(book, ct);
        await db.SaveChangesAsync(ct);

        return new BookDto(book.Id, book.Title, book.Author, book.Isbn.Value, book.TotalCopies, book.AvailableCopies);
    }
}