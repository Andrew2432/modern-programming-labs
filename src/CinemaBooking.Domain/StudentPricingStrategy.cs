using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaBooking.Domain;

public class StudentPricingStrategy : ITicketPricingStrategy
{
    public decimal CalculatePrice(decimal basePrice)
    {
        return basePrice * 0.5m; 
    }
}
