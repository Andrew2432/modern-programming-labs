namespace CinemaBooking.Domain.Models;

public record Seat
{
    public int Row { get; }
    public int Number { get; }

    public Seat(int row, int number)
    {
        if (row <= 0)
            throw new ArgumentOutOfRangeException(nameof(row), row, "Номер ряду має бути більшим за нуль.");
        if (number <= 0)
            throw new ArgumentOutOfRangeException(nameof(number), number, "Номер місця має бути більшим за нуль.");

        Row = row;
        Number = number;
    }
}
