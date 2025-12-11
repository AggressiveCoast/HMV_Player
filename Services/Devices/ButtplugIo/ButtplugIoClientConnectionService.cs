using System;
using System.Threading.Tasks;
using Buttplug.Client;
using Buttplug.Core;
using Buttplug.Core.Messages;
using HMV_Player.Controls;
using HMV_Player.MVVM.ViewModels;

namespace HMV_Player.Services.Devices.ButtplugIo;

public class ButtplugIoClientConnectionService : IButtplugIoClientConnectionService {

    private readonly NotificationContainerViewModel _notificationContainerViewModel;

    private ButtplugClient? _client;

    public ButtplugIoClientConnectionService(NotificationContainerViewModel notificationContainerViewModel) {
        _notificationContainerViewModel = notificationContainerViewModel;
    }

    public async Task ConnectToServer() {
        _client = new ButtplugClient("HMV Player");
        
        _client.DeviceAdded += ClientOnDeviceAdded;
        _client.DeviceRemoved += ClientOnDeviceRemoved;

        try {
            await _client.ConnectAsync(new ButtplugWebsocketConnector(new Uri("ws://127.0.0.1:12345")));
        }
        catch (Exception ex)
        {
            ErrorLogger.Log(ex);
            _notificationContainerViewModel.ShowNotification("Intiface Connection Failed", ex.Message, NotificationType.Warning);
            return;
        }
        
        _notificationContainerViewModel.ShowNotification("Intiface Connection Success", "", NotificationType.Info);

        _client.ScanningFinished += ClientOnScanningFinished;
        _client.ServerDisconnect += ClientOnServerDisconnect;

        await ScanDevices();

        await _client.StopScanningAsync();
        
        foreach (var device in _client.Devices)
        {
            Console.WriteLine($"- {device.Name}");
        }
    }

    private void ClientOnServerDisconnect(object? sender, EventArgs e) {
        _notificationContainerViewModel.ShowNotification("Intiface Disconnected", "", NotificationType.Info);
    }

    public async Task DisconnectFromServer() {
        try {
            await _client.DisconnectAsync();
        }
        catch (ButtplugException e) {
            _notificationContainerViewModel.ShowNotification("Disconnection Failed", e.Message, NotificationType.Warning);
        }

        _client = null;
    }

    private void ClientOnScanningFinished(object? sender, EventArgs e) {
        
    }

    private void ClientOnDeviceRemoved(object? sender, DeviceRemovedEventArgs e) {
        
    }

    private void ClientOnDeviceAdded(object? sender, DeviceAddedEventArgs e) {
        
    }

    public async Task ScanDevices() {
        await _client?.StartScanningAsync();
    }

    public ButtplugClientDevice[] GetDevices() {
        return _client?.Devices;
    }

    public bool IsConnected() {
        return _client?.Connected ?? false;
    }

    public ButtplugClient GetClient() {
        return  _client;
    }
}