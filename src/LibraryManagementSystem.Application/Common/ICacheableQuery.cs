using System;

namespace LibraryManagementSystem.Application.Common;

public interface ICacheableQuery
{
    string CacheKey { get; }
    TimeSpan? AbsoluteExpiration { get; }
}