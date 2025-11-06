using System;
using LibraryManagementSystem.Domain.Common;
using LibraryManagementSystem.Domain.ValueObjects;

namespace LibraryManagementSystem.Domain.Entities;

public class Book : Entity
{
    private Book() { }

    public string Title { get; private set; } = null!;
    public string Author { get; private set; } = null!;
    public Isbn Isbn { get; private set; } = null!;
    public int TotalCopies { get; private set; }
    public int AvailableCopies { get; private set; }
    public bool IsDeleted { get; private set; }

    public static Book Create(string title, string author, Isbn isbn, int totalCopies = 1)
    {
        if (totalCopies <= 0) throw new ArgumentOutOfRangeException(nameof(totalCopies));
        return new Book
        {
            Title = title.Trim(),
            Author = author.Trim(),
            Isbn = isbn,
            TotalCopies = totalCopies,
            AvailableCopies = totalCopies,
            IsDeleted = false
        };
    }

    public void Update(string title, string author, Isbn isbn, int newTotalCopies)
    {
        var borrowed = TotalCopies - AvailableCopies;
        if (newTotalCopies < borrowed)
            throw new InvalidOperationException("New total copies cannot be less than currently borrowed copies.");

        Title = title.Trim();
        Author = author.Trim();
        Isbn = isbn;

        TotalCopies = newTotalCopies;
        AvailableCopies = newTotalCopies - borrowed;
    }

    public void Borrow()
    {
        if (IsDeleted) throw new InvalidOperationException("Deleted book cannot be borrowed.");
        if (AvailableCopies <= 0) throw new InvalidOperationException("No available copies.");
        AvailableCopies--;
    }

    public void Return()
    {
        if (IsDeleted) throw new InvalidOperationException("Deleted book cannot be returned.");
        if (AvailableCopies >= TotalCopies) throw new InvalidOperationException("All copies are already in.");
        AvailableCopies++;
    }

    public void Delete()
    {
        if (IsDeleted) return;
        IsDeleted = true;
    }
}
