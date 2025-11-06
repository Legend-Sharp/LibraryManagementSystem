using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using LibraryManagementSystem.Application.Abstractions;
using LibraryManagementSystem.Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace LibraryManagementSystem.Application.Features.Members.Commands;

public sealed record CreateMemberCommand(string Name, string Email) : IRequest<Guid>;

public sealed class CreateMemberValidator : AbstractValidator<CreateMemberCommand>
{
    public CreateMemberValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200)
            .Must(n => n.Trim().Length > 0).WithMessage("Name cannot be whitespace.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress()
            .MaximumLength(200);
    }
}

public sealed class CreateMemberHandler(IAppDbContext db) : IRequestHandler<CreateMemberCommand, Guid>
{
    public async Task<Guid> Handle(CreateMemberCommand request, CancellationToken ct)
    {
        var normalizedEmail = request.Email.Trim();
        var normalizedName  = request.Name.Trim();

        var emailExists = await db.Members
            .AsNoTracking()
            .AnyAsync(m => m.Email == normalizedEmail, ct);
        
        if (emailExists)
            throw new InvalidOperationException("Email already registered.");

        var nameExists = await db.Members
            .AsNoTracking()
            .AnyAsync(m => m.Name.ToLower() == normalizedName.ToLower(), ct);
        
        if (nameExists)
            throw new InvalidOperationException("Name already in use.");

        var member = Member.Create(normalizedName, normalizedEmail);

        await db.Members.AddAsync(member, ct);
        await db.SaveChangesAsync(ct);

        return member.Id;
    }
}