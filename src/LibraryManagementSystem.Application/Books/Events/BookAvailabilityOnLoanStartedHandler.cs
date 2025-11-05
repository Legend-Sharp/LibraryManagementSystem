using LibraryManagementSystem.Application.Abstractions;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Domain.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Application.Books.Events;

public sealed class BookAvailabilityOnLoanStartedHandler : INotificationHandler<DomainEventNotification<LoanStartedDomainEvent>>
{
    private readonly IAppDbContext _db;
    public BookAvailabilityOnLoanStartedHandler(IAppDbContext db) => _db = db;

    public async Task Handle(DomainEventNotification<LoanStartedDomainEvent> notification, CancellationToken ct)
    {
        var e = notification.DomainEvent;

        var book = await _db.Books.FirstOrDefaultAsync(b => b.Id == e.BookId, ct)
                   ?? throw new KeyNotFoundException("Book not found for LoanStarted.");

        if (book.AvailableCopies <= 0)
            throw new InvalidOperationException("No available copies.");

        book.Borrow();
    }
}