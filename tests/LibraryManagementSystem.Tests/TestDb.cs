using LibraryManagementSystem.Infrastructure.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NSubstitute;

namespace LibraryManagementSystem.Tests;

public static class TestDb
{
    public static LibraryDbContext NewContext(string name)
    {
        var options = new DbContextOptionsBuilder<LibraryDbContext>()
            .UseInMemoryDatabase(name)
            .Options;
        
        var publisher = Substitute.For<IPublisher>();
        return new LibraryDbContext(options, publisher);
    }
}