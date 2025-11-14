using MobileDartsApp.Services.Online.Dtos.Player;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace MobileDartsApp.Services.Online.DataServices.Rest
{
    public class RestPlayerDataService : IRestPlayerDataService
    {
        private readonly HttpClient _httpClient;
        private readonly string _url;
        private readonly JsonSerializerOptions _jsonSerializeOptions;
        private readonly static string _baseAddress = DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:5016" : "https://localhost:7076"; //For emulator access
        //protected readonly static string _baseAddress = "http://192.168.1.8:5016"; //For local network access
        public RestPlayerDataService(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _url = $"{_baseAddress}/players";
            _jsonSerializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }
        public async Task<(bool, string)> RegisterPlayerAsync(PlayerDto playerDto)
        {
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
                return (false, "No internet access!");

            try
            {
                string json = JsonSerializer.Serialize(playerDto, _jsonSerializeOptions);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10)))
                {
                    HttpResponseMessage response = await _httpClient.PostAsync($"{_url}/register", content, cts.Token);

                    if (response.IsSuccessStatusCode)
                        return (true, "Registration successful");
                    else
                        return (false, "Non Http 2xx response during registration!");
                }
            }
            catch (TaskCanceledException)
            {
                return (false, "Registration request timed out.");
            }
            catch (HttpRequestException ex)
            {
                return (false, $"Server unreachable. Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return (false, $"Unexpected error during registration: {ex.Message}");
            }
        }
        public async Task<(bool, string)> LoginPlayerAsync(PlayerDto playerDto)
        {
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
                return (false, "No internet access!");

            try
            {
                string json = JsonSerializer.Serialize(playerDto, _jsonSerializeOptions);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10)))
                {
                    HttpResponseMessage response = await _httpClient.PostAsync($"{_url}/login", content, cts.Token);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseContent = await response.Content.ReadAsStringAsync();
                        var loginResponse = JsonSerializer.Deserialize<LoginResponseDto>(responseContent, _jsonSerializeOptions);

                        await SecureStorage.SetAsync("jwt_token", loginResponse.Token);
                        await SecureStorage.SetAsync("user_id", loginResponse.User.Id.ToString());
                        await SecureStorage.SetAsync("user_username", loginResponse.User.Username);
                        return (true, "Login successful");
                    }
                    else
                    {
                        return (false, "Non Http 2xx response during login!");
                    }
                }
            }
            catch (TaskCanceledException)
            {
                return (false, "Login request timed out.");
            }
            catch (HttpRequestException ex)
            {
                return (false, $"Server unreachable. Error: {ex.Message}");
            }
            catch (Exception ex)
            {
                return (false, $"Unexpected error during login: {ex.Message}");
            }
        }

        public async Task<List<PlayerDto>> GetAllPlayersAsync()
        {
            List<PlayerDto> players = new List<PlayerDto>();
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            {
                Debug.WriteLine("---> No internet access!");
                return players;
            }

            try
            {
                var token = await SecureStorage.GetAsync("jwt_token");
                if (string.IsNullOrEmpty(token))
                {
                    Debug.WriteLine("---> No user logged in, cannot fetch players.");
                    return players;
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await _httpClient.GetAsync($"{_url}");

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    players = JsonSerializer.Deserialize<List<PlayerDto>>(content, _jsonSerializeOptions);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    Logout();
                    throw new UnauthorizedAccessException("Token expired. Please log in again.");
                }
                else
                {
                    Debug.WriteLine("---> Non Http 2xx response!");
                }
            }
            catch (UnauthorizedAccessException)
            {
                throw;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Whoops exception occured: {ex.Message}");
                throw;
            }
            return players;
        }

        public async Task<PlayerDto?> GetCurrentUserAsync()
        {
            try
            {
                var idString = await SecureStorage.GetAsync("user_id");
                var username = await SecureStorage.GetAsync("user_username");

                if (string.IsNullOrEmpty(idString) || string.IsNullOrEmpty(username))
                {
                    return null;
                }

                return new PlayerDto
                {
                    Id = int.Parse(idString),
                    Username = username
                };
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error retrieving user data: {ex.Message}");
                return null;
            }
        }

        public void Logout()
        {
            SecureStorage.Remove("jwt_token");
            SecureStorage.Remove("user_id");
            SecureStorage.Remove("user_username");
        }


    }
}
