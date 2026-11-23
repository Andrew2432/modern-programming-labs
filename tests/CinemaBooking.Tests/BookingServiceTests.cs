using CinemaBooking.Domain;
using CinemaBooking.Domain.Exceptions;
using CinemaBooking.Domain.Models;
using CinemaBooking.Domain.Repositories;
using Xunit;

namespace CinemaBooking.Tests;

public class BookingServiceTests
{
   
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

    [Fact]
    public void PurchaseTicket_WhenSeatIsFree_ReturnsTicketForThatSeat()
    {
       
        var service = CreateService();
        var screening = CreateScreening();
        var seat = new Seat(1, 5);

        Ticket ticket = service.PurchaseTicket(screening, seat, 100m, price => price);

        Assert.Equal(seat, ticket.Seat);
        Assert.Equal(100m, ticket.Price);
        Assert.Single(screening.Tickets);
    }

    [Fact]
    public void PurchaseTicket_WhenSeatIsFree_DecreasesAvailableSeats()
    {
      
        var service = CreateService();
        var screening = CreateScreening();
        int seatsBefore = screening.AvailableSeats;

        service.PurchaseTicket(screening, new Seat(1, 5), 100m, price => price);

        Assert.Equal(seatsBefore - 1, screening.AvailableSeats);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-5)]
    public void PurchaseTicket_WhenSeatNumberIsZeroOrNegative_ThrowsArgumentOutOfRange(int seatNumber)
    {
        var service = CreateService();
        var screening = CreateScreening();

        Assert.Throws<ArgumentOutOfRangeException>(
            () => service.PurchaseTicket(screening, new Seat(1, seatNumber), 100m, price => price));

        Assert.Empty(screening.Tickets);
        Assert.Equal(10, screening.AvailableSeats);
    }

    [Theory]
    [InlineData(ScreeningStatus.Scheduled)]
    [InlineData(ScreeningStatus.Finished)]
    [InlineData(ScreeningStatus.Cancelled)]
    public void PurchaseTicket_WhenSalesAreClosed_ThrowsInvalidOperation(ScreeningStatus status)
    {
        
        var service = CreateService();
        var screening = CreateScreening(status);

        Assert.Throws<InvalidOperationException>(
            () => service.PurchaseTicket(screening, new Seat(1, 5), 100m, price => price));

        Assert.Empty(screening.Tickets);
    }
}
