using CinemaBooking.Domain.Events;

namespace CinemaBooking.Domain.Services;

public sealed class StatisticsService
{
    private readonly Dictionary<string, int> _ticketsByMovie = [];

    public int TotalTickets { get; private set; }
    public decimal TotalRevenue { get; private set; }
    public IReadOnlyDictionary<string, int> TicketsByMovie => _ticketsByMovie;

    public void Handle(object? sender, TicketPurchasedEventArgs e)
    {
        var title = e.Ticket.Screening.Movie.Title;
        TotalTickets++;
        TotalRevenue += e.Ticket.Price;
        _ticketsByMovie[title] = _ticketsByMovie.GetValueOrDefault(title) + 1;
    }
}
