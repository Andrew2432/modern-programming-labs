using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaBooking.Domain;

public enum ScreeningStatus
{
    Scheduled,
    SalesOpen,
    Started,
    Finished,
    Cancelled,
    Available,
    SoldOut
}
