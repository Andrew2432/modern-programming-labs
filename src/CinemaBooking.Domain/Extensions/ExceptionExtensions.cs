using System.Text.RegularExpressions;

namespace CinemaBooking.Domain.Extensions;

public static class ExceptionExtensions
{

    public static string GetFriendlyMessage(this Exception exception)
    {
        ArgumentNullException.ThrowIfNull(exception);
        var firstLine = exception.Message.Split('\n')[0].TrimEnd('\r');
        return Regex.Replace(firstLine, @"\s*\([^()]*\)$", "");
    }
}
