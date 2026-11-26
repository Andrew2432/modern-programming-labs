using CinemaBooking.App.Services;
using CinemaBooking.Domain;
using CinemaBooking.Domain.Exceptions;
using CinemaBooking.Domain.Models;
using CinemaBooking.Domain.Repositories;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace CinemaBooking.Tests;

public class TicketApplicationServiceTests
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

    private static TicketApplicationService CreateService(InMemoryReceiptSender receipts)
    {
        var bookingService = new BookingService(new InMemoryRepository<Ticket>(), _ => { });
        return new TicketApplicationService(
            bookingService, receipts, NullLogger<TicketApplicationService>.Instance);
    }

    [Fact]
    public void BookSeat_WhenSeatIsFree_SendsOneReceipt()
    {
        
        var receipts = new InMemoryReceiptSender();
        var service = CreateService(receipts);
        var screening = CreateScreening();

        service.BookSeat(screening, new Seat(1, 5), 100m);

        string message = Assert.Single(receipts.Messages);
        Assert.Contains("місце 5", message);
    }

    [Fact]
    public void BookSeat_WhenSeatIsAlreadyTaken_DoesNotSendSecondReceipt()
    {
        
        var receipts = new InMemoryReceiptSender();
        var service = CreateService(receipts);
        var screening = CreateScreening();
        service.BookSeat(screening, new Seat(1, 5), 100m);

        Assert.Throws<DomainRuleViolationException>(
            () => service.BookSeat(screening, new Seat(1, 5), 100m));

        Assert.Single(receipts.Messages);
        Assert.Equal(9, screening.AvailableSeats);
    }

    [Fact]
    public void BookSeat_WhenSalesAreClosed_DoesNotSendReceipt()
    {
        var receipts = new InMemoryReceiptSender();
        var service = CreateService(receipts);
        var screening = CreateScreening(ScreeningStatus.Scheduled);

        Assert.Throws<InvalidOperationException>(
            () => service.BookSeat(screening, new Seat(1, 5), 100m));

        Assert.Empty(receipts.Messages);
    }
}
