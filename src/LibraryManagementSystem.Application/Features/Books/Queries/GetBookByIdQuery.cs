using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LibraryManagementSystem.Application.Abstractions;
using LibraryManagementSystem.Application.Features.Books.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Application.Features.Books.Queries;

public sealed record GetBookByIdQuery(Guid Id) : IRequest<BookDto?>;

public sealed class GetBookByIdQueryHandler(IAppDbContext db) : IRequestHandler<GetBookByIdQuery, BookDto?>
{
    public async Task<BookDto?> Handle(GetBookByIdQuery r, CancellationToken ct)
    {
        var result = await db.Books.AsNoTracking()
            .Where(b => b.Id == r.Id)
            .Select(b => new BookDto(
                b.Id,
                b.Title,
                b.Author,
                b.Isbn.Value,
                b.TotalCopies,
                b.AvailableCopies,
                b.IsDeleted))
            .FirstOrDefaultAsync(ct);

        return result;
    }
}