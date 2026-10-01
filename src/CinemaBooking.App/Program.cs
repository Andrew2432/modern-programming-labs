using CinemaBooking.Domain;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.WriteLine("Modern Programming Course");
Console.WriteLine("Student: Борисенко Андрій Сергійович");
Console.WriteLine("Group: 301-ТК");
Console.WriteLine("Variant: 4");
Console.WriteLine("Domain: Cinema Booking");
Console.WriteLine("=====================================\n");

var movie = new Movie("Матриця", 136);
var hall = new Hall("Червоний зал", 50);
var screening = new Screening(movie, hall);

Console.WriteLine($"Створено сеанс: Фільм '{screening.Movie.Title}', Зал '{screening.Hall.Name}'\n");

ITicketPricingStrategy regularPricing = new RegularPricingStrategy();
ITicketPricingStrategy studentPricing = new StudentPricingStrategy();

decimal basePrice = 150m;

screening.ReserveSeat(1, basePrice, regularPricing);
screening.ReserveSeat(2, basePrice, studentPricing);

Console.WriteLine("Бронювання завершено. Список проданих квитків:");

foreach (var ticket in screening.Tickets)
{
    Console.WriteLine($"Місце: {ticket.SeatNumber}, Вартість квитка: {ticket.Price} грн");
}
