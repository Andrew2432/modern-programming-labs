using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CinemaBooking.Domain.Repositories;

public class InMemoryRepository<T>
{
    // Створюємо порожній список для зберігання наших об'єктів
    private readonly List<T> _items = [];

    // Метод для додавання елемента
    public void Add(T item)
    {
        _items.Add(item);
    }

    // Метод для видалення елемента
    public void Remove(T item)
    {
        _items.Remove(item);
    }

    // Метод для отримання всіх елементів 
    // Повертаємо IReadOnlyCollection, щоб список не можна було змінити ззовні
    public IReadOnlyCollection<T> GetAll()
    {
        return _items;
    }
}
