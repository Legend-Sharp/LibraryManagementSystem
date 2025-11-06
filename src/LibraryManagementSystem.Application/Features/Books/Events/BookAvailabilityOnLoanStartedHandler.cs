using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using LibraryManagementSystem.Application.Abstractions;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Domain.Events;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Application.Features.Books.Events;

public sealed class BookAvailabilityOnLoanStartedHandler(IAppDbContext db) : INotificationHandler<DomainEventNotification<LoanStartedDomainEvent>>
{
    public async Task Handle(DomainEventNotification<LoanStartedDomainEvent> notification, CancellationToken ct)
    {
        var e = notification.DomainEvent;

        var book = await db.Books.FirstOrDefaultAsync(b => b.Id == e.BookId, ct)
                   ?? throw new KeyNotFoundException("Book not found for LoanStarted.");

        if (book.AvailableCopies <= 0)
            throw new InvalidOperationException("No available copies.");

        book.Borrow();
    }
}