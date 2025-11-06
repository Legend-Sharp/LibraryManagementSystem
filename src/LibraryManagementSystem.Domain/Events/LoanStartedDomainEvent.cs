using System;
using LibraryManagementSystem.Domain.Common;

namespace LibraryManagementSystem.Domain.Events;

public sealed class LoanStartedDomainEvent : IDomainEvent
{
    public Guid LoanId { get; }
    public Guid MemberId { get; }
    public Guid BookId { get; }
    public DateTime BorrowedAt { get; }
    public DateTime? DueAt { get; }

    public LoanStartedDomainEvent(Guid loanId, Guid memberId, Guid bookId, DateTime borrowedAt, DateTime? dueAt)
    {
        LoanId = loanId;
        MemberId = memberId;
        BookId = bookId;
        BorrowedAt = borrowedAt;
        DueAt = dueAt;
    }
}