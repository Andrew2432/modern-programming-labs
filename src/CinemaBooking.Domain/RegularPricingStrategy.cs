using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaBooking.Domain;

public class RegularPricingStrategy : ITicketPricingStrategy
{
    public decimal CalculatePrice(decimal basePrice)
    {
        return basePrice; 
    }
}
