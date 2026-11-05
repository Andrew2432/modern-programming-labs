using CinemaBooking.Domain.Events;
using CinemaBooking.Domain.Models;
using CinemaBooking.Domain.Repositories;

namespace CinemaBooking.Domain;

public sealed class BookingService(InMemoryRepository<Ticket> tickets, Action<string> logger)
{
    public event EventHandler<TicketPurchasedEventArgs>? TicketPurchased;

    public void OpenSales(Screening screening)
    {
        screening.Status = ScreeningStatus.SalesOpen;
        logger($"Продаж квитків відкрито: {screening.Movie.Title}");
    }

    public void CancelScreening(Screening screening)
    {
        screening.Status = ScreeningStatus.Cancelled;
        logger($"Сеанс скасовано: {screening.Movie.Title}");
    }

    public Ticket PurchaseTicket(
        Screening screening, Seat seat, decimal basePrice, Func<decimal, decimal> priceModifier)
    {
        logger($"Спроба купівлі: {screening.Movie.Title}, ряд {seat.Row}, місце {seat.Number}");

        if (screening.Status is not (ScreeningStatus.SalesOpen or ScreeningStatus.Available))
        {
            logger($"Відмова: продаж на сеанс '{screening.Movie.Title}' недоступний.");
            throw new InvalidOperationException("Продаж квитків на цей сеанс недоступний.");
        }

        if (basePrice <= 0)
            throw new ArgumentException("Ціна квитка має бути більшою за нуль.", nameof(basePrice));

        var ticket = new Ticket(screening, seat, priceModifier(basePrice));

        screening.RegisterTicket(ticket); 
        tickets.Add(ticket);

        logger($"Квиток продано за {ticket.Price:F2} грн.");
        TicketPurchased?.Invoke(this, new TicketPurchasedEventArgs(ticket));
        return ticket;
    }
}
