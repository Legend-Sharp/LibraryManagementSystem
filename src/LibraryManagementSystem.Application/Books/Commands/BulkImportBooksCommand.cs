using FluentValidation;
using LibraryManagementSystem.Application.Abstractions;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.ValueObjects;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Application.Books.Commands;

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

public sealed class BulkImportBooksHandler : IRequestHandler<BulkImportBooksCommand, BulkImportResult>
{
    private readonly IAppDbContext _db;
    public BulkImportBooksHandler(IAppDbContext db) => _db = db;

    public async Task<BulkImportResult> Handle(BulkImportBooksCommand request, CancellationToken ct)
    {
        var errors = new List<string>();
        var toAdd = new List<Book>(request.Books.Count);

        // Pre-validate + build entities
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

        // Concurrency-safe batched insert (good perf with SQL Server)
        const int batchSize = 500;
        for (var i = 0; i < toAdd.Count; i += batchSize)
        {
            var batch = toAdd.Skip(i).Take(batchSize).ToList();
            await _db.Books.AddRangeAsync(batch, ct);
            await _db.SaveChangesAsync(ct);
        }

        return new(request.Books.Count, toAdd.Count, errors.Count, errors);
    }
}
