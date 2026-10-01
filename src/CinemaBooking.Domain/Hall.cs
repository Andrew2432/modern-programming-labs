using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaBooking.Domain;

public class Hall
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Name { get; set; }
    public int TotalSeats { get; set; }

    public Hall(string name, int totalSeats)
    {
        Name = name;
        TotalSeats = totalSeats;
    }
}
