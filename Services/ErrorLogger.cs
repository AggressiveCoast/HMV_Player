using System;
using System.IO;
using HMV_Player.Data;

namespace HMV_Player.Services;

public class ErrorLogger {
    private static readonly string _logPath;
    private static readonly object _exceptionLock = new();
    private static readonly object _messageLock = new();
    static ErrorLogger() {
        _logPath = HMVPlayerAppPaths.ErrorLogFilePath;

        Directory.CreateDirectory(Path.GetDirectoryName(_logPath)!);
        if (File.Exists(_logPath)) {
            File.Delete(_logPath);
        }
    }

    public static void Log(Exception ex, string? context = null)
    {
        lock (_exceptionLock)
        {
            File.AppendAllText(_logPath, $"{DateTime.Now} {context}\n{ex}\n");
        }
    }
    
    public static void Log(string message)
    {
        lock (_messageLock)
        {
            File.AppendAllText(_logPath, $"{DateTime.Now} {message}\n");
        }
    }
}