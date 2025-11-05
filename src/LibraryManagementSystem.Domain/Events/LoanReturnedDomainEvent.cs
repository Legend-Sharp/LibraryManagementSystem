using LibraryManagementSystem.Domain.Common;

namespace LibraryManagementSystem.Domain.Events;

public sealed class LoanReturnedDomainEvent : IDomainEvent
{
    public Guid LoanId { get; }
    public Guid MemberId { get; }
    public Guid BookId { get; }
    public DateTime ReturnedAt { get; }

    public LoanReturnedDomainEvent(Guid loanId, Guid memberId, Guid bookId, DateTime returnedAt)
    {
        LoanId = loanId;
        MemberId = memberId;
        BookId = bookId;
        ReturnedAt = returnedAt;
    }
}