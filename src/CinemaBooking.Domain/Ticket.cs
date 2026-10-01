using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaBooking.Domain;

public class Ticket
{
    public Guid Id { get; } = Guid.NewGuid();
    public int SeatNumber { get; }
    public decimal Price { get; }

    public Ticket(int seatNumber, decimal price)
    {
        SeatNumber = seatNumber;
        Price = price;
    }
}
