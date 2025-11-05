using FluentValidation;
using LibraryManagementSystem.Application.Abstractions;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.ValueObjects;
using MediatR;

namespace LibraryManagementSystem.Application.Features.Books.Commands;

public sealed record BulkImportBooksCommand(IReadOnlyList<BulkBook> Books) : IRequest<BulkImportResult>;

public sealed record BulkBook(string Title, string Author, string Isbn, int TotalCopies);
public sealed record BulkImportResult(int Processed, int Succeeded, int Failed, IReadOnlyList<string> Errors);

public sealed class BulkImportValidator : AbstractValidator<BulkImportBooksCommand>
{
    public BulkImportValidator()
    {
        RuleFor(x => x.Books).NotEmpty().Must(b => b.Count <= 5000)
            .WithMessage("Max 5000 per batch.");
    }
}

public sealed class BulkImportBooksHandler(IAppDbContext db) : IRequestHandler<BulkImportBooksCommand, BulkImportResult>
{
    public async Task<BulkImportResult> Handle(BulkImportBooksCommand request, CancellationToken ct)
    {
        var errors = new List<string>();
        var toAdd = new List<Book>(request.Books.Count);

        foreach (var item in request.Books)
        {
            try
            {
                toAdd.Add(Book.Create(item.Title, item.Author, Isbn.Create(item.Isbn), item.TotalCopies));
            }
            catch (Exception ex)
            {
                errors.Add($"{item.Title} ({item.Isbn}) -> {ex.Message}");
            }
        }

        const int batchSize = 500;
        for (var i = 0; i < toAdd.Count; i += batchSize)
        {
            var batch = toAdd.Skip(i).Take(batchSize).ToList();
            await db.Books.AddRangeAsync(batch, ct);
            await db.SaveChangesAsync(ct);
        }

        return new BulkImportResult(request.Books.Count, toAdd.Count, errors.Count, errors);
    }
}
