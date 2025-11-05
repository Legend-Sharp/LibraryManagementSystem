using LibraryManagementSystem.Domain.Common;
using MediatR;

namespace LibraryManagementSystem.Application.Common;

public interface INotificationEvent : IDomainEvent, INotification { }