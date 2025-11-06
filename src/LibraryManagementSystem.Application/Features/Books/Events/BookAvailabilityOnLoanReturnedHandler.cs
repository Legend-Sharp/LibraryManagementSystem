using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LibraryManagementSystem.Application.Abstractions;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Domain.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Application.Features.Books.Events;

public sealed class BookAvailabilityOnLoanReturnedHandler(IAppDbContext db) : INotificationHandler<DomainEventNotification<LoanReturnedDomainEvent>>
{
    public async Task Handle(DomainEventNotification<LoanReturnedDomainEvent> notification, CancellationToken ct)
    {
        var e = notification.DomainEvent;

        var book = await db.Books.FirstOrDefaultAsync(b => b.Id == e.BookId, ct)
                   ?? throw new KeyNotFoundException("Book not found for LoanReturned.");

        book.Return();
    }
}