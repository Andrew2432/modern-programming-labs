using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaBooking.Domain;

public class Movie
{
    public Guid Id { get; } = Guid.NewGuid();
    public string Title { get; set; }
    public int DurationMinutes { get; set; }

    public Movie(string title, int durationMinutes)
    {
        Title = title;
        DurationMinutes = durationMinutes;
    }
}
