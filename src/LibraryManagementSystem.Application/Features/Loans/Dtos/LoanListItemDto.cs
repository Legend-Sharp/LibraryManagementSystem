using System;

namespace LibraryManagementSystem.Application.Features.Loans.Dtos;

public sealed record LoanListItemDto(
    Guid Id,
    Guid MemberId,
    string MemberName,
    Guid BookId,
    string BookTitle,
    DateTime BorrowedAt,
    DateTime? DueAt,
    DateTime? ReturnedAt,
    bool IsActive);