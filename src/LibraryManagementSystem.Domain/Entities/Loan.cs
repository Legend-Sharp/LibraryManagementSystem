using System;
using LibraryManagementSystem.Domain.Common;
using LibraryManagementSystem.Domain.Events;

namespace LibraryManagementSystem.Domain.Entities;

public class Loan : Entity
{
    private Loan() { }

    public Guid MemberId { get; private set; }
    public Guid BookId { get; private set; }
    public DateTime BorrowedAt { get; private set; }
    public DateTime? ReturnedAt { get; private set; }
    public DateTime? DueAt { get; private set; }
    public bool IsActive => ReturnedAt is null;

    public static Loan Create(Guid memberId, Guid bookId, DateTime now, DateTime? dueAt = null)
    {
        var loan = new Loan { MemberId = memberId, BookId = bookId, BorrowedAt = now, DueAt = dueAt };
        loan.Raise(new LoanStartedDomainEvent(loan.Id, memberId, bookId, now, dueAt));
        return loan;
    }

    public void Return(DateTime now)
    {
        if (!IsActive) throw new InvalidOperationException("Loan already returned.");
        ReturnedAt = now;

        Raise(new LoanReturnedDomainEvent(Id, MemberId, BookId, now));
    }
}