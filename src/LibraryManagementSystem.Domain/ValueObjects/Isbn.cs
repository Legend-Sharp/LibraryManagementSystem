using System;
using System.Text.RegularExpressions;

namespace LibraryManagementSystem.Domain.ValueObjects;

public sealed class Isbn : IEquatable<Isbn>
{
    public string Value { get; }
    private Isbn(string value) => Value = value;

    public static Isbn Create(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw))
            throw new ArgumentException("ISBN required.", nameof(raw));

        var digits = Regex.Replace(raw, "[^0-9Xx]", "");
        if (!(digits.Length is 10 or 13))
            throw new ArgumentException("ISBN must be 10 or 13 digits.", nameof(raw));

        return new Isbn(digits.ToUpperInvariant());
    }

    public override string ToString() => Value;
    public bool Equals(Isbn? other) => other is not null && Value == other.Value;
    public override bool Equals(object? obj) => obj is Isbn other && Equals(other);
    public override int GetHashCode() => Value.GetHashCode();
}