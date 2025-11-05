using LibraryManagementSystem.Domain.Common;

namespace LibraryManagementSystem.Domain.Entities;

public class Member : Entity
{
    private Member() { }
    public string Name { get; private set; } = null!;
    public string Email { get; private set; } = null!;

    public static Member Create(string name, string email)
    {
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name required.", nameof(name));
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentException("Email required.", nameof(email));
        return new Member { Name = name.Trim(), Email = email.Trim() };
    }
}