using CinemaBooking.Domain.Models;

namespace CinemaBooking.Domain.Events;

public sealed class TicketPurchasedEventArgs(Ticket ticket) : EventArgs
{
    public Ticket Ticket { get; } = ticket;
}
