using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaBooking.Domain;

public interface ITicketPricingStrategy
{
    decimal CalculatePrice(decimal basePrice);
}
