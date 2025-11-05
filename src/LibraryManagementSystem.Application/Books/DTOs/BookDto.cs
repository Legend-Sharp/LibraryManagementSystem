namespace LibraryManagementSystem.Application.Books.DTOs;

public sealed record BookDto(Guid Id, string Title, string Author, string Isbn, int TotalCopies, int AvailableCopies);