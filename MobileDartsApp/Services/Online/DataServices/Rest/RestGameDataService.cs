using MobileDartsApp.Services.Online.Dtos.Game;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace MobileDartsApp.Services.Online.DataServices.Rest
{
    public class RestGameDataService : IRestGameDataService
    {
        private readonly HttpClient _httpClient;
        private readonly string _url;
        private readonly JsonSerializerOptions _json;
        private readonly string _baseAddress = DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:5016" : "https://localhost:7076"; //For emulator access
        //protected readonly static string _baseAddress = "http://192.168.1.8:5016"; //For local network access

        public RestGameDataService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _url = $"{_baseAddress}/games";
            _json = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        }

        public async Task<ThrowAppliedDto?> SubmitThrowAsync(string lobbyGUID, SubmitThrowDto dto)
        {
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            {
                Debug.WriteLine("---> No internet access!");
                return null;
            }

            var token = await SecureStorage.GetAsync("jwt_token");
            if (string.IsNullOrEmpty(token))
                throw new UnauthorizedAccessException("No user logged in.");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var content = new StringContent(JsonSerializer.Serialize(dto, _json), Encoding.UTF8, "application/json");
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(120));
            var resp = await _httpClient.PostAsync($"{_url}/{lobbyGUID}/throw", content, cts.Token);

            if (!resp.IsSuccessStatusCode)
            {
                var err = await resp.Content.ReadAsStringAsync();
                Debug.WriteLine($"---> Throw submit failed: {(int)resp.StatusCode} {resp.ReasonPhrase} - {err}");
                throw new HttpRequestException(err);
            }

            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<ThrowAppliedDto>(json, _json);
        }

        public async Task<GameStateDto?> GetStateAsync(string lobbyGUID)
        {
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            {
                Debug.WriteLine("---> No internet access!");
                return null;
            }

            var token = await SecureStorage.GetAsync("jwt_token");
            if (string.IsNullOrEmpty(token))
                throw new UnauthorizedAccessException("No user logged in.");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(120));
            var resp = await _httpClient.GetAsync($"{_url}/{lobbyGUID}/state", cts.Token);
            if (!resp.IsSuccessStatusCode) return null;

            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<GameStateDto>(json, _json);
        }

        public async Task<GameSummaryDto?> GetSummaryAsync(string lobbyGUID)
        {
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet) return null;

            var token = await SecureStorage.GetAsync("jwt_token");
            if (string.IsNullOrEmpty(token)) throw new UnauthorizedAccessException("No user logged in.");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));
            var resp = await _httpClient.GetAsync($"{_url}/{lobbyGUID}/summary", cts.Token);
            if (!resp.IsSuccessStatusCode) return null;

            var json = await resp.Content.ReadAsStringAsync();
            return JsonSerializer.Deserialize<GameSummaryDto>(json, _json);
        }

        public async Task<bool> ForfeitAsync(string lobbyGUID)
        {
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet) return false;

            var token = await SecureStorage.GetAsync("jwt_token");
            if (string.IsNullOrEmpty(token)) throw new UnauthorizedAccessException("No user logged in.");
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(20));
            var resp = await _httpClient.PostAsync($"{_url}/{lobbyGUID}/forfeit", content: null, cts.Token);

            return resp.IsSuccessStatusCode;
        }


    }
}
