namespace CinemaBooking.Domain.Pricing;

public static class PricingModifiers
{
    public static readonly Func<decimal, decimal> Regular = price => price;
    public static readonly Func<decimal, decimal> Student = price => price * 0.5m;
    public static readonly Func<decimal, decimal> Weekend = price => price * 1.2m;
}
