using CinemaBooking.Domain;
using CinemaBooking.Domain.Exceptions;
using CinemaBooking.Domain.Models;
using Microsoft.Extensions.Logging;

namespace CinemaBooking.App.Services;

public sealed class TicketApplicationService(
    BookingService bookingService,
    IReceiptSender receiptSender,
    ILogger<TicketApplicationService> logger)
{
    public Ticket BookSeat(Screening screening, Seat seat, decimal price)
    {
        ArgumentNullException.ThrowIfNull(screening);
        ArgumentNullException.ThrowIfNull(seat);

        logger.LogInformation("Booking seat {Row}-{Number} for {Movie}", seat.Row, seat.Number, screening.Movie.Title);

        Ticket ticket;
        try
        {
            ticket = bookingService.PurchaseTicket(screening, seat, price, p => p);
        }
        catch (DomainRuleViolationException ex) when (ex.Rule == DomainRules.SeatAlreadyTaken)
        {
            logger.LogWarning("Seat {Row}-{Number} is already taken", seat.Row, seat.Number);
            throw;   
        }

        receiptSender.Send($"Квиток: {screening.Movie.Title}, ряд {seat.Row}, місце {seat.Number}, {ticket.Price:F2} грн");
        logger.LogInformation("Ticket sold: seat {Row}-{Number}, price {Price}", seat.Row, seat.Number, ticket.Price);

        return ticket;
    }
}
