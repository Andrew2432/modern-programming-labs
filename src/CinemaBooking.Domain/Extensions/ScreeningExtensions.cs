using CinemaBooking.Domain.Models;

namespace CinemaBooking.Domain.Extensions;

public static class ScreeningExtensions
{
    public static bool IsAlmostFull(this Screening screening)
        => ScreeningPredicates.AlmostFull(screening);

    public static string GetSummary(this Screening screening)
        => $"{screening.Movie.Title} | {screening.Hall.Name} | " +
           $"{screening.GetScreeningStatusDescription()} | " +
           $"зайнято {screening.Tickets.Count}/{screening.Hall.TotalSeats} ({screening.GetOccupancy():P0})";

    public static double GetOccupancy(this Screening screening)
        => screening.Hall.TotalSeats == 0
            ? 0
            : (double)screening.Tickets.Count / screening.Hall.TotalSeats;
}
