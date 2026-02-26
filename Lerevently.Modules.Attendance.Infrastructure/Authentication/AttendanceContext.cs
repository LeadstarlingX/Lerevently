using Lerevently.Common.Application.Exceptions;
using Lerevently.Common.Infrastructure.Authentication;
using Lerevently.Modules.Attendance.Application.Abstractions.Authentication;
using Microsoft.AspNetCore.Http;

namespace Lerevently.Modules.Attendance.Infrastructure.Authentication;

internal sealed class AttendanceContext(IHttpContextAccessor httpContextAccessor) : IAttendanceContext
{
    public Guid AttendeeId => httpContextAccessor.HttpContext?.User.GetUserId() ??
                              throw new EventlyException("User identifier is unavailable");
}
