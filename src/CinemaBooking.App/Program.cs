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

moviesRepo.Add(m1); moviesRepo.Add(m2); moviesRepo.Add(m3);

var s1 = new Screening { Movie = m1, Hall = hall, Status = ScreeningStatus.Available };
var s2 = new Screening { Movie = m2, Hall = hall, Status = ScreeningStatus.Available };
var s3 = new Screening { Movie = m3, Hall = hall, Status = ScreeningStatus.SoldOut };

screeningsRepo.Add(s1); screeningsRepo.Add(s2); screeningsRepo.Add(s3);

for (int i = 1; i <= 15; i++)
{
    var currentScreening = i <= 5 ? s1 : (i <= 10 ? s2 : s3);
    int startRow = i > 10 ? 2 : 1;
    int startSeat = i > 10 ? i - 10 : i;

    ticketsRepo.Add(new Ticket(currentScreening, new Seat(startRow, startSeat), i <= 10 ? 150m : 200m));
}

bool isRunning = true;
while (isRunning)
{
    Console.WriteLine("\n=============================");
    Console.WriteLine("    КІНОТЕАТР 'ПОЛІТЕХНІКА'  ");
    Console.WriteLine("=============================");
    Console.WriteLine($"В базі: Фільмів ({moviesRepo.GetAll().Count}), Сеансів ({screeningsRepo.GetAll().Count}), Квитків ({ticketsRepo.GetAll().Count})");
    Console.WriteLine("1. Показати всі сеанси");
    Console.WriteLine("2. Купити квиток (Матриця місць)");
    Console.WriteLine("3. Показати LINQ-статистику");
    Console.WriteLine("0. Вийти");
    Console.Write("Оберіть дію: ");

    var choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            Console.WriteLine("\n--- ДОСТУПНІ СЕАНСИ ---");
            var allScreenings = screeningsRepo.GetAll().ToList();
            for (int i = 0; i < allScreenings.Count; i++)
            {
                Console.WriteLine($"{i + 1}. Фільм: {allScreenings[i].Movie.Title} | Статус: {allScreenings[i].Status}");
            }
            break;

        case "2":
            Console.WriteLine("\n--- КУПІВЛЯ КВИТКА ---");
            var availableScreenings = screeningsRepo.GetAll().ToList();

            Console.WriteLine("Оберіть номер сеансу:");
            for (int i = 0; i < availableScreenings.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {availableScreenings[i].Movie.Title}");
            }
            Console.Write("Ваш вибір: ");

            if (int.TryParse(Console.ReadLine(), out int screeningIndex) && screeningIndex > 0 && screeningIndex <= availableScreenings.Count)
            {
                var selectedScreening = availableScreenings[screeningIndex - 1];

                ShowSeatMatrix(selectedScreening, ticketsRepo);

                Console.Write("Введіть номер ряду (1-10): ");
                int.TryParse(Console.ReadLine(), out int row);

                Console.Write("Введіть номер місця (1-10): ");
                int.TryParse(Console.ReadLine(), out int seatNumber);

                if (row < 1 || row > 10 || seatNumber < 1 || seatNumber > 10)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n[ПОМИЛКА] Такого місця не існує. Оберіть ряд та місце від 1 до 10.");
                    Console.ResetColor();
                    break;
                }

                bool isSeatTaken = ticketsRepo.GetAll()
                    .Any(t => t.Screening == selectedScreening && t.Seat.Row == row && t.Seat.Number == seatNumber);

                if (isSeatTaken)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\n[ПОМИЛКА] Це місце вже продано! Будь ласка, оберіть інше (зелене).");
                    Console.ResetColor();
                }
                else
                {
                   
                    var newTicket = new Ticket(selectedScreening, new Seat(row, seatNumber), 150m);
                    ticketsRepo.Add(newTicket);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"\n[УСПІХ] Квиток на фільм '{selectedScreening.Movie.Title}' (Ряд {row}, Місце {seatNumber}) успішно придбано!");
                    Console.ResetColor();
                }
            }
            else
            {
                Console.WriteLine("\n[ПОМИЛКА] Невірний номер сеансу.");
            }
            break;

        case "3":
            ShowLinqStatistics(screeningsRepo, ticketsRepo);
            break;

        case "0":
            isRunning = false;
            Console.WriteLine("\nДякуємо за використання системи! До побачення.");
            break;

        default:
            Console.WriteLine("\n[ПОМИЛКА] Невідома команда. Спробуйте ще раз.");
            break;
    }
}

static void ShowSeatMatrix(Screening screening, InMemoryRepository<Ticket> ticketsRepo)
{
    Console.WriteLine($"\n--- ПЛАН ЗАЛУ: {screening.Movie.Title} ---");
    Console.WriteLine("          Е К Р А Н          \n");

    var takenSeats = ticketsRepo.GetAll()
        .Where(t => t.Screening == screening)
        .Select(t => t.Seat)
        .ToList();

    Console.Write("   ");
    for (int s = 1; s <= 10; s++) Console.Write($"{s,3}");
    Console.WriteLine();

    for (int r = 1; r <= 10; r++)
    {
        Console.Write($"{r,2} "); 
        for (int s = 1; s <= 10; s++)
        {
          
            bool isTaken = takenSeats.Any(seat => seat.Row == r && seat.Number == s);

            if (isTaken)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("[X]");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write("[ ]");
                Console.ResetColor();
            }
        }
        Console.WriteLine();
    }
    Console.WriteLine("-----------------------------------");
    Console.ForegroundColor = ConsoleColor.Green;
    Console.Write("[ ] - Вільно  |  ");
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("[X] - Зайнято");
    Console.ResetColor();
    Console.WriteLine("-----------------------------------\n");
}

static void ShowLinqStatistics(InMemoryRepository<Screening> screeningsRepo, InMemoryRepository<Ticket> ticketsRepo)
{
    Console.WriteLine("\n=== РЕЗУЛЬТАТИ LINQ-ЗАПИТІВ ===");
    var allScreenings = screeningsRepo.GetAll();
    var allTickets = ticketsRepo.GetAll();

    var openScreenings = allScreenings.Where(s => s.Status == ScreeningStatus.Available).ToList();
    Console.WriteLine($"1. Сеансів відкрито (Where): {openScreenings.Count}");

    var totalRevenue = allTickets.Sum(t => t.Price);
    Console.WriteLine($"2. Загальний дохід (Sum): {totalRevenue} грн");

    if (allTickets.Any())
    {
        var averageTicketPrice = allTickets.Average(t => t.Price);
        Console.WriteLine($"3. Середня ціна квитка (Average): {averageTicketPrice:F2} грн");
    }

    var ticketsPerMovie = allTickets
        .GroupBy(t => t.Screening.Movie.Title)
        .Select(g => new { MovieTitle = g.Key, Count = g.Count() }).ToList();
    Console.WriteLine("4. Кількість квитків на кожен фільм:");
    foreach (var item in ticketsPerMovie) Console.WriteLine($"   - {item.MovieTitle}: {item.Count} шт.");

    var topScreenings = allTickets
        .GroupBy(t => t.Screening)
        .Select(g => new { Movie = g.Key.Movie.Title, Revenue = g.Sum(t => t.Price) })
        .OrderByDescending(x => x.Revenue).Take(5).ToList();
    Console.WriteLine("5. Топ сеансів за доходом:");
    foreach (var item in topScreenings) Console.WriteLine($"   - {item.Movie}: {item.Revenue} грн");

    var allSoldSeats = allTickets.GroupBy(t => t.Screening).SelectMany(group => group.Select(ticket => ticket.Seat)).ToList();
    Console.WriteLine($"6. Проданих місць загалом (SelectMany): {allSoldSeats.Count}");

    var top3Movies = allTickets
        .GroupBy(t => t.Screening.Movie.Title)
        .Select(g => new { MovieTitle = g.Key, Sold = g.Count() })
        .OrderByDescending(x => x.Sold).Take(3).ToList();
    Console.WriteLine("7. Топ-3 фільми:");
    foreach (var item in top3Movies) Console.WriteLine($"   - {item.MovieTitle} ({item.Sold} квитків)");

    bool hasSoldOut = allScreenings.Any(s => s.Status == ScreeningStatus.SoldOut);
    Console.WriteLine($"8. Повністю розпродані сеанси (Any): {(hasSoldOut ? "Так" : "Ні")}");

    var popularScreenings = allTickets
        .GroupBy(t => t.Screening)
        .Select(g => new { Movie = g.Key.Movie.Title, Occupancy = (double)g.Count() / 100.0 })
        .Where(x => x.Occupancy > 0.8).ToList();
    Console.WriteLine($"9. Сеансів із заповненістю > 80%: {popularScreenings.Count}");

    var movieStatsDict = allTickets.GroupBy(t => t.Screening.Movie.Title).ToDictionary(g => g.Key, g => g.Count());
    Console.WriteLine($"10. Dictionary створено. Записів: {movieStatsDict.Count}");
}