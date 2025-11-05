using LibraryManagementSystem.Application.Abstractions;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Domain.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Application.Books.Events;

public sealed class BookAvailabilityOnLoanReturnedHandler : INotificationHandler<DomainEventNotification<LoanReturnedDomainEvent>>
{
    private readonly IAppDbContext _db;
    public BookAvailabilityOnLoanReturnedHandler(IAppDbContext db) => _db = db;

    public async Task Handle(DomainEventNotification<LoanReturnedDomainEvent> notification, CancellationToken ct)
    {
        var e = notification.DomainEvent;

        var book = await _db.Books.FirstOrDefaultAsync(b => b.Id == e.BookId, ct)
                   ?? throw new KeyNotFoundException("Book not found for LoanReturned.");

        book.Return();
    }
}