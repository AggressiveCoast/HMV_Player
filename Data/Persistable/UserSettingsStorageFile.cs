using Avalonia.Controls;

namespace HMV_Player.Data.Persistable;

public class UserSettingsStorageFile {

    public int DefaultVolume { get; set; } = 50;
    public double UserWindowWidth { get; set; }
    public double UserWindowHeight { get; set; }
    
    public WindowState WindowState { get; set; } = WindowState.Normal;
    public bool IsMaximized { get; set; }
}