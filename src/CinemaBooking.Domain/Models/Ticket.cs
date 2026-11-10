namespace CinemaBooking.Domain.Models;

public record Ticket
{
    public Screening Screening { get; }
    public Seat Seat { get; }
    public decimal Price { get; }

    public Ticket(Screening screening, Seat seat, decimal price)
    {
        ArgumentNullException.ThrowIfNull(screening);
        ArgumentNullException.ThrowIfNull(seat);
        if (price <= 0)
            throw new ArgumentOutOfRangeException(nameof(price), price, "Ціна квитка має бути більшою за нуль.");

        Screening = screening;
        Seat = seat;
        Price = price;
    }
}
