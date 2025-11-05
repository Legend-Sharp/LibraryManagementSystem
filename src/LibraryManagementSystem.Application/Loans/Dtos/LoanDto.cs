namespace LibraryManagementSystem.Application.Loans.DTOs;

public sealed record LoanDto(Guid Id, Guid MemberId, Guid BookId, string BookTitle, DateTime BorrowedAt, DateTime? DueAt, bool IsActive);