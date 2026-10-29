using System.Text;
using CinemaBooking.Domain;
using CinemaBooking.Domain.Repositories;

Console.OutputEncoding = Encoding.UTF8;

var moviesRepo = new InMemoryRepository<Movie>();
var screeningsRepo = new InMemoryRepository<Screening>();
var ticketsRepo = new InMemoryRepository<Ticket>();

var hall = new Hall("Main Hall", 100);

var m1 = new Movie { Title = "Dune", Description = "Sci-Fi", DurationMinutes = 155 };
var m2 = new Movie { Title = "Matrix", Description = "Action", DurationMinutes = 136 };
var m3 = new Movie { Title = "Avatar", Description = "Sci-Fi", DurationMinutes = 162 };

moviesRepo.Add(m1);
moviesRepo.Add(m2);
moviesRepo.Add(m3);

var s1 = new Screening { Movie = m1, Hall = hall, Status = ScreeningStatus.Available };
var s2 = new Screening { Movie = m2, Hall = hall, Status = ScreeningStatus.Available };
var s3 = new Screening { Movie = m3, Hall = hall, Status = ScreeningStatus.SoldOut };

screeningsRepo.Add(s1);
screeningsRepo.Add(s2);
screeningsRepo.Add(s3);

for (int i = 1; i <= 15; i++)
{
    var currentScreening = i <= 5 ? s1 : (i <= 10 ? s2 : s3);
    var seat = new Seat(Row: 1, Number: i);
    decimal price = i <= 10 ? 150m : 200m;

    ticketsRepo.Add(new Ticket(currentScreening, seat, price));
}

Console.WriteLine($"Успішно завантажено:");
Console.WriteLine($"Фільмів: {moviesRepo.GetAll().Count}");
Console.WriteLine($"Сеансів: {screeningsRepo.GetAll().Count}");
Console.WriteLine($"Квитків: {ticketsRepo.GetAll().Count}");

Console.WriteLine("\nРЕЗУЛЬТАТИ LINQ-ЗАПИТІВ \n");

var allScreenings = screeningsRepo.GetAll();
var allTickets = ticketsRepo.GetAll();

var openScreenings = allScreenings.Where(s => s.Status == ScreeningStatus.Available).ToList();
Console.WriteLine($"1. Сеансів відкрито для продажу: {openScreenings.Count}");

var totalRevenue = allTickets.Sum(t => t.Price);
Console.WriteLine($"2. Загальний дохід (Revenue): {totalRevenue} грн");

var averageTicketPrice = allTickets.Average(t => t.Price);
Console.WriteLine($"3. Середня ціна квитка: {averageTicketPrice:F2} грн");

var ticketsPerMovie = allTickets
    .GroupBy(t => t.Screening.Movie.Title)
    .Select(g => new { MovieTitle = g.Key, Count = g.Count() })
    .ToList();

Console.WriteLine("4. Кількість квитків на кожен фільм:");
foreach (var item in ticketsPerMovie)
{
    Console.WriteLine($"   - {item.MovieTitle}: {item.Count} шт.");
}

var topScreenings = allTickets
    .GroupBy(t => t.Screening)
    .Select(g => new { Movie = g.Key.Movie.Title, Revenue = g.Sum(t => t.Price) })
    .OrderByDescending(x => x.Revenue)
    .Take(5)
    .ToList();

Console.WriteLine("5. Топ сеансів за доходом:");
foreach (var item in topScreenings)
{
    Console.WriteLine($"   - {item.Movie}: {item.Revenue} грн");
}

var allSoldSeats = allTickets
    .GroupBy(t => t.Screening)
    .SelectMany(group => group.Select(ticket => ticket.Seat))
    .ToList();
Console.WriteLine($"6. Загальна кількість проданих місць (через SelectMany): {allSoldSeats.Count}");

var top3Movies = allTickets
    .GroupBy(t => t.Screening.Movie.Title)
    .Select(g => new { MovieTitle = g.Key, Sold = g.Count() })
    .OrderByDescending(x => x.Sold)
    .Take(3)
    .ToList();
Console.WriteLine("7. Топ-3 фільми за популярністю:");
foreach (var item in top3Movies)
{
    Console.WriteLine($"   - {item.MovieTitle} ({item.Sold} квитків)");
}

bool hasSoldOut = allScreenings.Any(s => s.Status == ScreeningStatus.SoldOut);
Console.WriteLine($"8. Чи є повністю розпродані сеанси (Any): {(hasSoldOut ? "Так" : "Ні")}");

var popularScreenings = allTickets
    .GroupBy(t => t.Screening)
    .Select(g => new { Movie = g.Key.Movie.Title, Occupancy = (double)g.Count() / 100.0 })
    .Where(x => x.Occupancy > 0.8)
    .ToList();
Console.WriteLine($"9. Сеансів із заповненістю > 80%: {popularScreenings.Count}");

Dictionary<string, int> movieStatsDict = allTickets
    .GroupBy(t => t.Screening.Movie.Title)
    .ToDictionary(g => g.Key, g => g.Count());

Console.WriteLine("10. Словник (Dictionary) фільмів та їхніх продажів створено успішно.");