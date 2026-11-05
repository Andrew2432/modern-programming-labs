using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaBooking.Domain;

public class Movie
{
    public Guid Id { get; } = Guid.NewGuid();
    
    public required string Title { get; init; } 
    public required int DurationMinutes { get; init; } 
    
    public string? Description { get; set; } 

    public string GetMovieCategory() => DurationMinutes switch
    {
        < 90 => "Short",
        <= 150 => "Standard",
        _ => "Long"
    };
}