using System.Threading;
using System.Threading.Tasks;
using LibraryManagementSystem.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Application.Abstractions;

public interface IAppDbContext
{
    DbSet<Book> Books { get; }
    DbSet<Member> Members { get; }
    DbSet<Loan> Loans { get; }
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}