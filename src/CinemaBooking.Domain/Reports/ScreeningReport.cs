using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaBooking.Domain.Reports;

public sealed record ScreeningReport(
    Guid ScreeningId,
    string MovieTitle,
    DateTime StartsAt,
    int TicketsSold,
    decimal Revenue);
