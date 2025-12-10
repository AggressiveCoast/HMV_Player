using System;
using System.IO;

namespace HMV_Player.Data;

public class HMVPlayerAppPaths {
    private static readonly string RoamingAppDir = 
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "HMV_Player");

    public static readonly string CacheDir = Path.Combine(RoamingAppDir, "Cache");
    public static readonly string ConfigDir = Path.Combine(RoamingAppDir, "Config");

    public static readonly string ThumbNailCache = Path.Combine(CacheDir, "Thumbnails");
    
    public static readonly string ErrorLogFilePath = Path.Combine(RoamingAppDir, "Logs",  "ErrorLog.txt");

    // Ensure folders exist
    static HMVPlayerAppPaths()
    {
        Directory.CreateDirectory(RoamingAppDir);
        Directory.CreateDirectory(CacheDir);
        Directory.CreateDirectory(ConfigDir);
        Directory.CreateDirectory(ThumbNailCache);
    }
}