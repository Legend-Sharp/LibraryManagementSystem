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

    public static Book Create(string title, string author, Isbn isbn, int totalCopies = 1)
    {
        if (totalCopies <= 0) throw new ArgumentOutOfRangeException(nameof(totalCopies));
        return new Book
        {
            Title = title.Trim(),
            Author = author.Trim(),
            Isbn = isbn,
            TotalCopies = totalCopies,
            AvailableCopies = totalCopies
        };
    }

    public void Borrow()
    {
        if (AvailableCopies <= 0) throw new InvalidOperationException("No available copies.");
        AvailableCopies--;
    }

    public void Return()
    {
        if (AvailableCopies >= TotalCopies) throw new InvalidOperationException("All copies are already in.");
        AvailableCopies++;
    }
}