using MobileDartsApp.Services.Online.Dtos.Lobby;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace MobileDartsApp.Services.Online.DataServices.Rest
{
    public class RestLobbyDataService : IRestLobbyDataService
    {

        private readonly HttpClient _httpClient;
        private readonly SettingsService _settingsService;
        private readonly string _url;
        private readonly JsonSerializerOptions _jsonSerializeOptions;
        protected readonly static string _baseAddress = DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:5016" : "https://localhost:7076"; //For emulator access
        //protected readonly static string _baseAddress = "http://192.168.1.8:5016"; //For local network access
        public RestLobbyDataService(HttpClient httpClient, SettingsService settingsService)
        {
            _httpClient = httpClient;
            _settingsService = settingsService;
            _url = $"{_baseAddress}/lobbies";
            _jsonSerializeOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        }

        public async Task<List<LobbyDto>> GetAllLobbies()
        {
            List<LobbyDto> lobbies = new List<LobbyDto>();
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            {
                Debug.WriteLine("---> No internet access!");
                return lobbies;
            }

            try
            {
                var token = await SecureStorage.GetAsync("jwt_token");
                if (string.IsNullOrEmpty(token))
                {
                    Debug.WriteLine("---> No user logged in, cannot fetch lobbies.");
                    throw new UnauthorizedAccessException("No user logged in.");
                }
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                HttpResponseMessage response = await _httpClient.GetAsync(_url);

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    lobbies = JsonSerializer.Deserialize<List<LobbyDto>>(content, _jsonSerializeOptions);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    Debug.WriteLine("---> Unauthorized access. Token may be expired.");
                    throw new UnauthorizedAccessException("Token expired. Please log in again.");
                }
                else
                {
                    Debug.WriteLine($"---> Non Http 2xx response! Status: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Whoops exception occurred: {ex.Message}");
                throw;
            }

            return lobbies;
        }

        public async Task<LobbyDto> JoinLobby(string lobbyGUID)
        {
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            {
                Debug.WriteLine("---> No internet access!");
                return null;
            }

            try
            {
                var token = await SecureStorage.GetAsync("jwt_token");
                if (string.IsNullOrEmpty(token))
                {
                    throw new UnauthorizedAccessException("No user logged in.");
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await _httpClient.PostAsync($"{_url}/{lobbyGUID}/join", null);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    var lobbyDto = JsonSerializer.Deserialize<LobbyDto>(responseContent, _jsonSerializeOptions);
                    Debug.WriteLine($"---> Successfully joined lobby with GUID: {lobbyGUID}");
                    return lobbyDto;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException("Token expired. Please log in again.");
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"---> Failed to join lobby! Status: {response.StatusCode}, Error: {errorContent}");
                    throw new Exception(errorContent);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Whoops exception occurred: {ex.Message}");
                throw;
            }
        }

        public async Task<LobbyDto> CreateLobby()
        {
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            {
                Debug.WriteLine("---> No internet access!");
                return null;
            }

            try
            {
                var token = await SecureStorage.GetAsync("jwt_token");
                if (string.IsNullOrEmpty(token))
                {
                    throw new UnauthorizedAccessException("No user logged in.");
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var settings = _settingsService.CurrentSettings;
                var content = JsonSerializer.Serialize(new CreateLobbyDto
                {
                    LobbyTitle = "My Lobby",
                    LobbyCreator = await SecureStorage.GetAsync("user_username"),
                    Settings = settings
                }, _jsonSerializeOptions);

                var httpContent = new StringContent(content, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _httpClient.PostAsync(_url, httpContent);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    var lobbyDto = JsonSerializer.Deserialize<LobbyDto>(responseContent, _jsonSerializeOptions);
                    Debug.WriteLine($"---> Successfully created lobby with GUID: {lobbyDto.LobbyGUID}");
                    return lobbyDto;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException("Token expired. Please log in again.");
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"---> Failed to create lobby! Status: {response.StatusCode}, Error: {errorContent}");
                    throw new Exception(errorContent);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Whoops exception occurred: {ex.Message}");
                throw;
            }
        }

        public async Task<LobbyDto?> GetLobbyStatusAsync(string lobbyGUID)
        {
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            {
                Debug.WriteLine("---> No internet access!");
                return null;
            }

            try
            {
                var token = await SecureStorage.GetAsync("jwt_token");
                if (string.IsNullOrEmpty(token))
                {
                    Debug.WriteLine("---> No user logged in, cannot fetch lobby status.");
                    throw new UnauthorizedAccessException("No user logged in.");
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await _httpClient.GetAsync($"{_url}/{lobbyGUID}/status");

                if (response.IsSuccessStatusCode)
                {
                    string content = await response.Content.ReadAsStringAsync();
                    var lobby = JsonSerializer.Deserialize<LobbyDto>(content, _jsonSerializeOptions);
                    Debug.WriteLine($"---> Successfully fetched lobby status: {lobbyGUID}");
                    return lobby;
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    Debug.WriteLine("---> Unauthorized access. Token may be expired.");
                    throw new UnauthorizedAccessException("Token expired. Please log in again.");
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"---> Failed to fetch lobby status! Status: {response.StatusCode}, Error: {errorContent}");
                    throw new Exception($"Failed to fetch lobby status: {errorContent}");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Whoops exception occurred: {ex.Message}");
                throw;
            }
        }

        public async Task UpdateReadyStatusAsync(string lobbyGUID)
        {
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            {
                Debug.WriteLine("---> No internet access!");
                return;
            }

            try
            {
                var token = await SecureStorage.GetAsync("jwt_token");
                if (string.IsNullOrEmpty(token))
                {
                    throw new UnauthorizedAccessException("No user logged in.");
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await _httpClient.PostAsync($"{_url}/{lobbyGUID}/ready", null);

                if (response.IsSuccessStatusCode)
                {
                    Debug.WriteLine($"---> Successfully updated ready status for lobby: {lobbyGUID}");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException("Token expired. Please log in again.");
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"---> Failed to update ready status! Status: {response.StatusCode}, Error: {errorContent}");
                    throw new Exception(errorContent);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Whoops exception occurred: {ex.Message}");
                throw;
            }
        }

        public async Task StartGameAsync(string lobbyGUID)
        {
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            {
                Debug.WriteLine("---> No internet access!");
                return;
            }

            try
            {
                var token = await SecureStorage.GetAsync("jwt_token");
                if (string.IsNullOrEmpty(token))
                {
                    throw new UnauthorizedAccessException("No user logged in.");
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await _httpClient.PostAsync($"{_url}/{lobbyGUID}/start", null);

                if (response.IsSuccessStatusCode)
                {
                    Debug.WriteLine($"---> Successfully started game for lobby: {lobbyGUID}");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException("Token expired. Please log in again.");
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"---> Failed to start game! Status: {response.StatusCode}, Error: {errorContent}");
                    throw new Exception(errorContent);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Whoops exception occurred: {ex.Message}");
                throw;
            }
        }

        public async Task LeaveLobbyAsync(string lobbyGUID)
        {
            if (Connectivity.Current.NetworkAccess != NetworkAccess.Internet)
            {
                Debug.WriteLine("---> No internet access!");
                return;
            }

            try
            {
                var token = await SecureStorage.GetAsync("jwt_token");
                if (string.IsNullOrEmpty(token))
                {
                    throw new UnauthorizedAccessException("No user logged in.");
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                HttpResponseMessage response = await _httpClient.PostAsync($"{_url}/{lobbyGUID}/leave", null);

                if (response.IsSuccessStatusCode)
                {
                    Debug.WriteLine($"---> Successfully left lobby: {lobbyGUID}");
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    throw new UnauthorizedAccessException("Token expired. Please log in again.");
                }
                else
                {
                    string errorContent = await response.Content.ReadAsStringAsync();
                    Debug.WriteLine($"---> Failed to leave lobby! Status: {response.StatusCode}, Error: {errorContent}");
                    throw new Exception(errorContent);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Whoops exception occurred: {ex.Message}");
                throw;
            }
        }
    }
}
