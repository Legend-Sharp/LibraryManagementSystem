using FluentAssertions;
using LibraryManagementSystem.Application.Members.Commands;
using LibraryManagementSystem.Application.Common;
using LibraryManagementSystem.Application.Members.DTOs;
using LibraryManagementSystem.Application.Members.Queries;
using LibraryManagementSystem.Domain.Entities;

namespace LibraryManagementSystem.Tests.Members;

public class MemberTests
{
    [Fact]
    public void Invalid_input_fails()
    {
        var v = new CreateMemberValidator();
        var r = v.Validate(new CreateMemberCommand("", "not-email"));
        r.IsValid.Should().BeFalse();
    }

    [Fact]
    public void Valid_input_passes()
    {
        var v = new CreateMemberValidator();
        var r = v.Validate(new CreateMemberCommand("Alice", "alice@example.com"));
        r.IsValid.Should().BeTrue();
    }
    
    [Fact]
    public async Task Returns_paged_members()
    {
        using var db = TestDb.NewContext(nameof(Returns_paged_members));

        await db.Members.AddRangeAsync(
            Member.Create("Alice", "a@ex.com"),
            Member.Create("Bob", "b@ex.com"),
            Member.Create("Carol", "c@ex.com"));
        await db.SaveChangesAsync();

        var handler = new GetMembersQueryHandler(db);
        PagedResult<MemberDto> res = await handler.Handle(new GetMembersQuery(1, 2, null), default);

        res.Items.Should().HaveCount(2);
        res.TotalCount.Should().Be(3);
    }

    [Fact]
    public async Task Applies_search_filter()
    {
        using var db = TestDb.NewContext(nameof(Applies_search_filter));
        await db.Members.AddRangeAsync(
            Member.Create("Alice", "a@ex.com"),
            Member.Create("Bob", "b@ex.com"));
        await db.SaveChangesAsync();

        var handler = new GetMembersQueryHandler(db);
        var res = await handler.Handle(new GetMembersQuery(1, 20, "Ali"), default);

        res.Items.Should().ContainSingle(m => m.Name == "Alice");
    }
}