using System.Threading.Tasks;
using Buttplug.Client;

namespace HMV_Player.Services.Devices.ButtplugIo;

public interface IButtplugIoClientConnectionService {

    public Task ConnectToServer();
    public Task DisconnectFromServer();

    public Task ScanDevices();
    public ButtplugClientDevice[]  GetDevices();
    
    public bool IsConnected();

    public ButtplugClient GetClient();
}