using CinemaBooking.Domain.Events;

namespace CinemaBooking.Domain.Services;

public sealed class ReceiptService(Action<string> output)
{
    public void Handle(object? sender, TicketPurchasedEventArgs e)
    {
        var t = e.Ticket;
        output("--------- ЧЕК ---------");
        output($"Фільм: {t.Screening.Movie.Title}");
        output($"Ряд {t.Seat.Row}, місце {t.Seat.Number}");
        output($"Сума: {t.Price:F2} грн");
        output("-----------------------");
    }
}
