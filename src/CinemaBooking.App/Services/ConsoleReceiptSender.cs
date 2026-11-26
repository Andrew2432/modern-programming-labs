namespace CinemaBooking.App.Services;

public sealed class ConsoleReceiptSender : IReceiptSender
{
    public void Send(string message)
    {
        Console.WriteLine($"[ЧЕК] {message}");
    }
}
