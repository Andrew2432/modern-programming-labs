using System.Text.RegularExpressions;

namespace CinemaBooking.Domain.Extensions;

public static class ExceptionExtensions
{
    /// <summary>
    /// Повідомлення винятку без службових доважків .NET
    /// («(Parameter 'x')», «Actual value was ...») для виводу користувачу.
    /// </summary>
    public static string GetFriendlyMessage(this Exception exception)
    {
        ArgumentNullException.ThrowIfNull(exception);
        var firstLine = exception.Message.Split('\n')[0].TrimEnd('\r');
        return Regex.Replace(firstLine, @"\s*\([^()]*\)$", "");
    }
}
