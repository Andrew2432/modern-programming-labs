using System.Net.Http.Json;

namespace CinemaBooking.App.Services;

public sealed class ExternalUserService(HttpClient client)
{
    public async Task<ExternalUser?> GetUserAsync(int id, CancellationToken token)
    {
        return await client.GetFromJsonAsync<ExternalUser>($"users/{id}", token);
    }
}
