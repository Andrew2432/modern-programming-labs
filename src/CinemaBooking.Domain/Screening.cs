using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaBooking.Domain;

public class Screening
{
    public Guid Id { get; } = Guid.NewGuid();
    public required Movie Movie { get; init; }
    public required Hall Hall { get; init; }
    
    public ScreeningStatus Status { get; set; } = ScreeningStatus.Scheduled;

    private readonly List<Ticket> _tickets = new();
    public IReadOnlyCollection<Ticket> Tickets => _tickets;

    public int AvailableSeats => Hall.TotalSeats - _tickets.Count;

    public string GetScreeningStatusDescription() => Status switch
    {
        ScreeningStatus.Scheduled => "Заплановано",
        ScreeningStatus.SalesOpen => "Продаж відкрито",
        ScreeningStatus.Started => "Розпочато",
        ScreeningStatus.Finished => "Завершено",
        ScreeningStatus.Cancelled => "Скасовано",
        _ => "Невідомо"
    };

    public bool IsSoldOut() => this switch
    {
        { Status: ScreeningStatus.SalesOpen, AvailableSeats: 0 } => true,
        _ => false
    };

    public void ReserveSeat(int seatNumber, decimal basePrice, ITicketPricingStrategy pricing)
    {
        if (Status != ScreeningStatus.SalesOpen)
            throw new InvalidOperationException("Бронювання недоступне. Продаж ще не відкрито або вже завершено.");

        if (basePrice <= 0)
            throw new ArgumentException("Ціна квитка має бути більшою за нуль.");

        if (_tickets.Any(t => t.Seat.Number == seatNumber))
            throw new InvalidOperationException("Це місце вже продане.");

        decimal finalPrice = pricing.CalculatePrice(basePrice);
        var ticket = new Ticket(this, new Seat(1, seatNumber), finalPrice);
        _tickets.Add(ticket);
    }
}
