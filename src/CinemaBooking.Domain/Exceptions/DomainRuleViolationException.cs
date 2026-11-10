namespace CinemaBooking.Domain.Exceptions;

/// <summary>Коди бізнес-правил, які можуть бути порушені.</summary>
public static class DomainRules
{
    public const string SeatAlreadyTaken = "SeatAlreadyTaken";
}

/// <summary>
/// Порушення бізнес-правила предметної області. Успадковує InvalidOperationException,
/// тому універсальні обробники продовжують працювати, але на відміну від нього
/// містить машинозчитуваний код правила (Rule), за яким можна відрізнити
/// «місце зайняте» від «продаж закрито» без розбору тексту повідомлення.
/// </summary>
public class DomainRuleViolationException : InvalidOperationException
{
    public string Rule { get; }

    public DomainRuleViolationException(string rule, string message) : base(message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rule);
        Rule = rule;
    }
}
