using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CinemaBooking.Domain.Exceptions;
using CinemaBooking.Domain.Pricing;

namespace CinemaBooking.Domain.Models;

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
        ScreeningStatus.Available => "Доступно",
        ScreeningStatus.SoldOut => "Розпродано",
        _ => "Невідомо"
    };

    public bool IsSoldOut() => this switch
    {
        { Status: ScreeningStatus.SalesOpen, AvailableSeats: 0 } => true,
        _ => false
    };

    /// <summary>Кидає InvalidOperationException, якщо продаж квитків на сеанс недоступний.</summary>
    public void EnsureSalesOpen()
    {
        switch (Status)
        {
            case ScreeningStatus.SalesOpen:
            case ScreeningStatus.Available:
                return;
            case ScreeningStatus.Scheduled:
                throw new InvalidOperationException("Продаж на цей сеанс ще не відкрито.");
            case ScreeningStatus.Started:
            case ScreeningStatus.Finished:
                throw new InvalidOperationException("Сеанс уже розпочато або завершено, продаж закрито.");
            case ScreeningStatus.Cancelled:
                throw new InvalidOperationException("Сеанс скасовано.");
            case ScreeningStatus.SoldOut:
                throw new InvalidOperationException("Усі квитки на цей сеанс розпродано.");
            default:
                throw new InvalidOperationException("Продаж на цей сеанс недоступний.");
        }
    }

    public void RegisterTicket(Ticket ticket)
    {
        // Спочатку всі перевірки, і лише потім зміна стану.
        ArgumentNullException.ThrowIfNull(ticket);

        if (!ReferenceEquals(ticket.Screening, this))
            throw new ArgumentException("Квиток належить іншому сеансу.", nameof(ticket));

        if (!Hall.HasSeat(ticket.Seat))
            throw new ArgumentOutOfRangeException(nameof(ticket), ticket.Seat,
                $"У залі '{Hall.Name}' немає місця: ряд {ticket.Seat.Row}, місце {ticket.Seat.Number}.");

        if (_tickets.Any(t => t.Seat == ticket.Seat))
            throw new DomainRuleViolationException(DomainRules.SeatAlreadyTaken,
                $"Місце вже зайняте: ряд {ticket.Seat.Row}, місце {ticket.Seat.Number}.");

        _tickets.Add(ticket);
    }

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
