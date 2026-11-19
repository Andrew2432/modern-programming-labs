using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using CinemaBooking.Domain.Reports;

namespace CinemaBooking.App.Services;

public sealed class ScreeningReportStorage
{
    public async Task SaveAsync(
        string filePath,
        IReadOnlyCollection<ScreeningReport> reports,
        bool writeIndented = true)
    {
        try
        {
            string? directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var options = new JsonSerializerOptions
            {
                WriteIndented = writeIndented
            };

            string json = JsonSerializer.Serialize(reports, options);
            await File.WriteAllTextAsync(filePath, json);

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[OK] Звіт успішно збережено у файл: {filePath}");
            Console.ResetColor();
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ПОМИЛКА ДОСТУПУ] Немає прав на запис файлу: {ex.Message}");
            Console.ResetColor();
        }
        catch (IOException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ПОМИЛКА ВВОДУ-ВИВОДУ] Не вдалося записати файл звіту: {ex.Message}");
            Console.ResetColor();
        }
    }

    public async Task<List<ScreeningReport>> LoadAsync(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("Файл звіту не знайдено.", filePath);
            }

            string json = await File.ReadAllTextAsync(filePath);
            var restored = JsonSerializer.Deserialize<List<ScreeningReport>>(json);

            if (restored is null)
            {
                throw new JsonException("Файл JSON містить null замість списку звітів.");
            }

            return restored;
        }
        catch (FileNotFoundException ex)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[УВАГА] Файл звіту відсутній ({ex.FileName}). Спочатку виконайте експорт.");
            Console.ResetColor();
            return [];
        }
        catch (JsonException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ПОМИЛКА JSON] Пошкоджена структура файлу звіту: {ex.Message}");
            Console.ResetColor();
            return [];
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ПОМИЛКА ДОСТУПУ] Немає прав на читання файлу: {ex.Message}");
            Console.ResetColor();
            return [];
        }
        catch (IOException ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ПОМИЛКА ВВОДУ-ВИВОДУ] Помилка при читанні файлу: {ex.Message}");
            Console.ResetColor();
            return [];
        }
    }
}