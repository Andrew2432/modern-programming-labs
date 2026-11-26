using CinemaBooking.Domain;
using CinemaBooking.Domain.Models;

namespace CinemaBooking.App.Services;

public sealed class TicketApplicationService(
    BookingService bookingService,
    IReceiptSender receiptSender)
{
    public Ticket BookSeat(Screening screening, Seat seat, decimal price)
    {
        
        Ticket ticket = bookingService.PurchaseTicket(screening, seat, price, p => p);

        receiptSender.Send($"Квиток: {screening.Movie.Title}, ряд {seat.Row}, місце {seat.Number}, {ticket.Price:F2} грн");

        return ticket;
    }
}
