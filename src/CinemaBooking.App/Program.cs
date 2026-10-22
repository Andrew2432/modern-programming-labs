using System;
using CinemaBooking.Domain;

Console.OutputEncoding = System.Text.Encoding.UTF8;

var movie = new Movie 
{ 
    Title = "Матриця", 
    DurationMinutes = 136 
};

var hall = new Hall("Зал 1", 50);

var screening = new Screening 
{ 
    Movie = movie, 
    Hall = hall,
    Status = ScreeningStatus.SalesOpen 
};

ITicketPricingStrategy studentPricing = new StudentPricingStrategy();

// бронювання
screening.ReserveSeat(2, 100m, studentPricing);

Console.WriteLine($"Фільм: {screening.Movie.Title} ({screening.Movie.GetMovieCategory()})");
Console.WriteLine($"Статус сеансу: {screening.GetScreeningStatusDescription()}");

foreach (var ticket in screening.Tickets)
{
    Console.WriteLine($"Квиток: Місце {ticket.SeatNumber}, Ціна {ticket.Price}");
}