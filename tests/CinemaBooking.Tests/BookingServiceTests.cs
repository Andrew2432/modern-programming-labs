using CinemaBooking.Domain;
using CinemaBooking.Domain.Exceptions;
using CinemaBooking.Domain.Models;
using CinemaBooking.Domain.Repositories;
using Xunit;

namespace CinemaBooking.Tests;

public class BookingServiceTests
{
    // Кожен тест створює власні дані, тому тести не залежать один від одного.
    private static Screening CreateScreening(ScreeningStatus status = ScreeningStatus.SalesOpen)
    {
        return new Screening
        {
            Movie = new Movie { Title = "Test Movie", DurationMinutes = 120 },
            Hall = new Hall("Test Hall", 10),
            Status = status
        };
    }

    private static BookingService CreateService()
    {
        return new BookingService(new InMemoryRepository<Ticket>(), _ => { });
    }

    // 1. Успішне бронювання вільного місця.
    [Fact]
    public void PurchaseTicket_WhenSeatIsFree_ReturnsTicketForThatSeat()
    {
        // Arrange
        var service = CreateService();
        var screening = CreateScreening();
        var seat = new Seat(1, 5);

        // Act
        Ticket ticket = service.PurchaseTicket(screening, seat, 100m, price => price);

        // Assert
        Assert.Equal(seat, ticket.Seat);
        Assert.Equal(100m, ticket.Price);
        Assert.Single(screening.Tickets);
    }
}
