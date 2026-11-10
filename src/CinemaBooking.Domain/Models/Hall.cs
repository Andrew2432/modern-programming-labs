namespace CinemaBooking.Domain.Models;

public class Hall
{
    private string _name = string.Empty;
    private int _totalSeats;

    public Guid Id { get; } = Guid.NewGuid();

    public string Name
    {
        get => _name;
        set
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);
            _name = value;
        }
    }

    public int TotalSeats
    {
        get => _totalSeats;
        set
        {
            if (value <= 0)
                throw new ArgumentOutOfRangeException(nameof(value), value, "Кількість місць у залі має бути більшою за нуль.");
            _totalSeats = value;
        }
    }

    public int SeatsPerRow { get; }

    public int Rows => (TotalSeats + SeatsPerRow - 1) / SeatsPerRow;

    public Hall(string name, int totalSeats, int seatsPerRow = 10)
    {
        if (seatsPerRow <= 0)
            throw new ArgumentOutOfRangeException(nameof(seatsPerRow), seatsPerRow, "Кількість місць у ряду має бути більшою за нуль.");

        SeatsPerRow = seatsPerRow;
        Name = name;
        TotalSeats = totalSeats;
    }

    /// <summary>Чи існує таке місце в цьому залі.</summary>
    public bool HasSeat(Seat seat)
    {
        ArgumentNullException.ThrowIfNull(seat);
        return seat.Number <= SeatsPerRow
            && (seat.Row - 1) * SeatsPerRow + seat.Number <= TotalSeats;
    }
}
