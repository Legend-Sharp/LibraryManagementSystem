using System;
using FluentAssertions;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.ValueObjects;
using Xunit;

namespace LibraryManagementSystem.Tests.Books;

public class BookTests
{
    [Fact]
    public void Borrow_decrements_available()
    {
        var book = Book.Create("SQRS", "Luka", Isbn.Create("9780321125217"), 2);
        book.Borrow();
        book.AvailableCopies.Should().Be(1);
    }

    [Fact]
    public void Borrow_when_none_available_throws()
    {
        var book = Book.Create("X", "Y", Isbn.Create("0123456789"), 1);
        book.Borrow();
        var act = () => book.Borrow();
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Return_increments_available()
    {
        var book = Book.Create("Z", "K", Isbn.Create("1234567890"), 1);
        book.Borrow();
        book.Return();
        book.AvailableCopies.Should().Be(1);
    }
}