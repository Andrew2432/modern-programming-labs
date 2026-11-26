namespace CinemaBooking.App.Services;

public sealed class InMemoryReceiptSender : IReceiptSender
{
    public List<string> Messages { get; } = [];

    public void Send(string message)
    {
        Messages.Add(message);
    }
}
