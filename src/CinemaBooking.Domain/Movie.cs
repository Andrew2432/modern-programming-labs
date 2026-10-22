using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaBooking.Domain;

public class Movie
{
    public Guid Id { get; } = Guid.NewGuid();
    
    // required обязывает задать значение при создании, а init запрещает его менять потом
    public required string Title { get; init; } 
    public required int DurationMinutes { get; init; } 
    
    // string? означает, что описание может быть пустым (null)
    public string? Description { get; set; } 

    // Relational pattern для распределения фильмов по длительности
    public string GetMovieCategory() => DurationMinutes switch
    {
        < 90 => "Short",
        <= 150 => "Standard",
        _ => "Long"
    };
}