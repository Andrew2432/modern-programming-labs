using CinemaBooking.Domain.Models;

namespace CinemaBooking.Domain.Extensions;

public static class ScreeningPredicates
{
    public const double AlmostFullThreshold = 0.8;

    public static readonly Predicate<Screening> AlmostFull =
        screening => screening.GetOccupancy() >= AlmostFullThreshold;
}
