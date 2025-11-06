using System.Threading.Tasks;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Infrastructure.Persistence;

public static class SeedData
{
    public static async Task InitializeAsync(LibraryDbContext db)
    {
        var hasAny = await db.Books.AnyAsync() || await db.Members.AnyAsync();
        if (hasAny) return;

        var b1 = Book.Create("Clean Code", "Robert C. Martin", Isbn.Create("0132350882"), 5);
        var b2 = Book.Create("Domain-Driven Design", "Eric Evans", Isbn.Create("0321125215"), 3);
        var b3 = Book.Create("The Pragmatic Programmer", "Andrew Hunt", Isbn.Create("020161622X"), 4);

        var m1 = Member.Create("Alice Johnson", "alice@example.com");
        var m2 = Member.Create("Bob Smith", "bob@example.com");

        await db.Books.AddRangeAsync(b1, b2, b3);
        await db.Members.AddRangeAsync(m1, m2);
        await db.SaveChangesAsync();
    }
}