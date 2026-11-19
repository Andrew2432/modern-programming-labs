namespace CinemaBooking.Domain.Exceptions;

public static class DomainRules
{
    public const string SeatAlreadyTaken = "SeatAlreadyTaken";
}

public class DomainRuleViolationException : InvalidOperationException
{
    public string Rule { get; }

    public DomainRuleViolationException(string rule, string message) : base(message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rule);
        Rule = rule;
    }
}
