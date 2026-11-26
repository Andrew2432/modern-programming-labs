using System.Net.Http.Json;
using CinemaBooking.App.Models;

namespace CinemaBooking.App.Services;

public sealed class ExternalUserService(HttpClient client)
{
    public async Task<ExternalUser?> GetUserAsync(int id, CancellationToken token)
    {
        return await client.GetFromJsonAsync<ExternalUser>($"users/{id}", token);
    }
}
