using Microsoft.AspNetCore.Http.Connections;
using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileDartsApp.Services.Online.DataServices.SignalR
{
    public abstract class SignalRHubServiceBase
    {
        protected readonly HubConnection _connection;
        public bool IsConnected => _connection.State == HubConnectionState.Connected;
        public event Action<bool>? ConnectionStateChanged;
        protected readonly static string _baseAddress = DeviceInfo.Platform == DevicePlatform.Android ? "http://10.0.2.2:5016" : "https://localhost:7076"; //For emulator access
        //protected readonly static string _baseAddress = "http://192.168.1.8:5016"; //For local network access
        protected SignalRHubServiceBase(string hubPath)
        {
            _connection = new HubConnectionBuilder()
                .WithUrl($"{_baseAddress}/{hubPath}")
                .WithAutomaticReconnect(new[]
                {
                    TimeSpan.Zero,
                    TimeSpan.FromSeconds(5),
                    TimeSpan.FromSeconds(10),
                    TimeSpan.FromSeconds(20)
                })
                .Build();

            _connection.Reconnecting += ex =>
            {
                Debug.WriteLine($"Reconnecting: {ex?.Message}");
                ConnectionStateChanged?.Invoke(false);
                return Task.CompletedTask;
            };

            _connection.Reconnected += id =>
            {
                Debug.WriteLine($"Reconnected: {id}");
                ConnectionStateChanged?.Invoke(true);
                return Task.CompletedTask;
            };

            _connection.Closed += ex =>
            {
                Debug.WriteLine($"Closed: {ex?.Message}");
                ConnectionStateChanged?.Invoke(false);
                return Task.CompletedTask;
            };
        }

        public async Task StartConnectionAsync()
        {
            try
            {
                switch (_connection.State)
                {
                    case HubConnectionState.Connected:
                        ConnectionStateChanged?.Invoke(true);
                        return;

                    case HubConnectionState.Connecting:
                    case HubConnectionState.Reconnecting:
                        await _connection.StopAsync();
                        goto case HubConnectionState.Disconnected;

                    case HubConnectionState.Disconnected:
                        using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(10)))
                        {
                            await _connection.StartAsync(cts.Token);
                            Debug.WriteLine("SignalR Connection started");
                            ConnectionStateChanged?.Invoke(true);
                        }
                        return;

                    default:
                        ConnectionStateChanged?.Invoke(false);
                        return;
                }
            }
            catch (OperationCanceledException)
            {
                Debug.WriteLine("SignalR connect timeout.");
                ConnectionStateChanged?.Invoke(false);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"SignalR connection error: {ex.Message}");
                ConnectionStateChanged?.Invoke(false);
            }
        }

        public async Task StopConnectionAsync()
        {
            try
            {
                if (_connection.State != HubConnectionState.Disconnected)
                {
                    await _connection.StopAsync();
                    Debug.WriteLine("SignalR connection stopped.");
                    ConnectionStateChanged?.Invoke(false);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"SignalR stop error: {ex.Message}");
            }
        }
    }
}
