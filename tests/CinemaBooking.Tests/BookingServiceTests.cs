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

    [Fact]
    public void PurchaseTicket_WhenSeatIsAlreadyTaken_ThrowsAndKeepsState()
    {
        
        var service = CreateService();
        var screening = CreateScreening();
        service.PurchaseTicket(screening, new Seat(1, 3), 100m, price => price);

        var exception = Assert.Throws<DomainRuleViolationException>(
            () => service.PurchaseTicket(screening, new Seat(1, 3), 100m, price => price));

        Assert.Equal(DomainRules.SeatAlreadyTaken, exception.Rule);

        Assert.Single(screening.Tickets);
        Assert.Equal(9, screening.AvailableSeats);
    }

    [Fact]
    public void TicketPurchased_IsRaisedOnce_AfterSuccessfulSale()
    {
        
        var service = CreateService();
        var screening = CreateScreening();
        int eventCount = 0;
        service.TicketPurchased += (_, _) => eventCount++;

        service.PurchaseTicket(screening, new Seat(1, 5), 100m, price => price);

        Assert.Equal(1, eventCount);
    }

    [Fact]
    public void TicketPurchased_IsNotRaised_WhenSaleFails()
    {
    
        var service = CreateService();
        var screening = CreateScreening(ScreeningStatus.Scheduled);
        int eventCount = 0;
        service.TicketPurchased += (_, _) => eventCount++;

        Assert.Throws<InvalidOperationException>(
            () => service.PurchaseTicket(screening, new Seat(1, 5), 100m, price => price));

        Assert.Equal(0, eventCount);
    }

    [Fact]
    public void TicketPurchased_NotifiesBothSubscribers()
    {
       
        var service = CreateService();
        var screening = CreateScreening();
        int firstCount = 0;
        int secondCount = 0;
        service.TicketPurchased += (_, _) => firstCount++;
        service.TicketPurchased += (_, _) => secondCount++;

        service.PurchaseTicket(screening, new Seat(1, 5), 100m, price => price);

        Assert.Equal(1, firstCount);
        Assert.Equal(1, secondCount);
    }
}
