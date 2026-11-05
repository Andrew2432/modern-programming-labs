using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaBooking.Domain.Repositories;

public class InMemoryRepository<T>
{
   
    private readonly List<T> _items = [];

    public void Add(T item)
    {
        _items.Add(item);
    }

    public void Remove(T item)
    {
        _items.Remove(item);
    }

    public IReadOnlyCollection<T> GetAll()
    {
        return _items;
    }
}
