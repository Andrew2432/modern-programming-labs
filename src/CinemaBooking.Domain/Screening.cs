using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaBooking.Domain;

public class Screening
{
    public Guid Id { get; } = Guid.NewGuid();

    public Movie Movie { get; }
    public Hall Hall { get; }

    public bool IsStarted { get; private set; }
    public bool IsFinished { get; private set; }

    private readonly List<Ticket> _tickets = new();
    public IReadOnlyCollection<Ticket> Tickets => _tickets;

    public Screening(Movie movie, Hall hall)
    {
        Movie = movie;
        Hall = hall;
    }

    public void ReserveSeat(int seatNumber, decimal basePrice, ITicketPricingStrategy pricing)
    {
        if (IsStarted || IsFinished)
            throw new InvalidOperationException("Неможливо забронювати квиток на сеанс, що вже почався.");

        if (basePrice <= 0)
            throw new ArgumentException("Ціна квитка має бути більшою за нуль."); 

        if (_tickets.Any(t => t.SeatNumber == seatNumber))
            throw new InvalidOperationException("Це місце вже продане."); 

        decimal finalPrice = pricing.CalculatePrice(basePrice);

        var ticket = new Ticket(seatNumber, finalPrice);
        _tickets.Add(ticket);
    }

    public void CancelReservation(int seatNumber)
    {
        if (IsStarted)
            throw new InvalidOperationException("Після початку сеансу квитки не повертаються."); 

        var ticket = _tickets.FirstOrDefault(t => t.SeatNumber == seatNumber);
        if (ticket != null)
        {
            _tickets.Remove(ticket);
        }
    }

    public void Start()
    {
        IsStarted = true;
    }

    public void Finish()
    {
        IsFinished = true;
    }
}
