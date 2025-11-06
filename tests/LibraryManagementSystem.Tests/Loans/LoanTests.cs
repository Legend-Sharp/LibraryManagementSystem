using System;
using System.Threading.Tasks;
using FluentAssertions;
using LibraryManagementSystem.Application.Features.Loans.Queries;
using LibraryManagementSystem.Domain.Entities;
using LibraryManagementSystem.Domain.ValueObjects;
using Xunit;

namespace LibraryManagementSystem.Tests.Loans;

public class LoanTests
{
    [Fact]
    public async Task Filters_by_member_and_active()
    {
        using var db = TestDb.NewContext(nameof(Filters_by_member_and_active));

        var m1 = Member.Create("M1", "m1@ex.com");
        var m2 = Member.Create("M2", "m2@ex.com");
        var b1 = Book.Create("B1", "A", Isbn.Create("1111111111"), 2);
        var b2 = Book.Create("B2", "A", Isbn.Create("2222222222"), 2);
        await db.Members.AddRangeAsync(m1, m2);
        await db.Books.AddRangeAsync(b1, b2);

        // loans
        var l1 = Loan.Create(m1.Id, b1.Id, DateTime.UtcNow);
        var l2 = Loan.Create(m1.Id, b2.Id, DateTime.UtcNow);
        l2.Return(DateTime.UtcNow.AddHours(1));
        var l3 = Loan.Create(m2.Id, b1.Id, DateTime.UtcNow);

        await db.Loans.AddRangeAsync(l1, l2, l3);
        await db.SaveChangesAsync();

        var handler = new GetLoansQueryHandler(db);

        var pageForM1Active = await handler.Handle(new GetLoansQuery(1, 20, m1.Id, null, true), default);
        pageForM1Active.Items.Should().OnlyContain(x => x.MemberId == m1.Id && x.IsActive);

        var pageForM1All = await handler.Handle(new GetLoansQuery(1, 20, m1.Id, null, null), default);
        pageForM1All.Items.Should().HaveCount(2);
    }
}