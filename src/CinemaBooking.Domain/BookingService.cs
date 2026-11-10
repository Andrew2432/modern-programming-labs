using CinemaBooking.Domain.Events;
using CinemaBooking.Domain.Extensions;
using CinemaBooking.Domain.Models;
using CinemaBooking.Domain.Repositories;

namespace CinemaBooking.Domain;

public sealed class BookingService(InMemoryRepository<Ticket> tickets, Action<string> logger)
{
    public event EventHandler<TicketPurchasedEventArgs>? TicketPurchased;

    public void OpenSales(Screening screening)
    {
        ArgumentNullException.ThrowIfNull(screening);

        if (screening.Status != ScreeningStatus.Scheduled)
            throw new InvalidOperationException("Продаж можна відкрити лише для запланованого сеансу.");

        screening.Status = ScreeningStatus.SalesOpen;
        logger($"Продаж квитків відкрито: {screening.Movie.Title}");
    }

    public void CancelScreening(Screening screening)
    {
        ArgumentNullException.ThrowIfNull(screening);

        if (screening.Status is ScreeningStatus.Started or ScreeningStatus.Finished)
            throw new InvalidOperationException("Не можна скасувати сеанс, який розпочато або завершено.");

        if (screening.Status == ScreeningStatus.Cancelled)
            throw new InvalidOperationException("Сеанс уже скасовано.");

        screening.Status = ScreeningStatus.Cancelled;
        logger($"Сеанс скасовано: {screening.Movie.Title}");
    }

    public Ticket PurchaseTicket(
        Screening screening, Seat seat, decimal basePrice, Func<decimal, decimal> priceModifier)
    {
        ArgumentNullException.ThrowIfNull(screening);
        ArgumentNullException.ThrowIfNull(seat);
        ArgumentNullException.ThrowIfNull(priceModifier);

        logger($"Спроба купівлі: {screening.Movie.Title}, ряд {seat.Row}, місце {seat.Number}");

        Ticket ticket;
        try
        {
            // 1. Усі перевірки. Стан сеансу та репозиторію ще не змінено.
            if (basePrice <= 0)
                throw new ArgumentOutOfRangeException(nameof(basePrice), basePrice, "Ціна квитка має бути більшою за нуль.");

            screening.EnsureSalesOpen();
            ticket = new Ticket(screening, seat, priceModifier(basePrice));

            // 2. Зміна стану (всередині ще раз перевіряє зал і зайняті місця).
            screening.RegisterTicket(ticket);
        }
        catch (Exception ex) when (ex is ArgumentException or InvalidOperationException)
        {
            logger($"Відмова: {ex.GetFriendlyMessage()}");
            throw; // throw; зберігає початковий стек викликів
        }

        tickets.Add(ticket);
        logger($"Квиток продано за {ticket.Price:F2} грн.");

        // 3. Подія лише після успішної зміни стану.
        TicketPurchased?.Invoke(this, new TicketPurchasedEventArgs(ticket));
        return ticket;
    }
}
