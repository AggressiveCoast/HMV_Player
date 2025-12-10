using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Ports;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using HMV_Player.Controls;
using HMV_Player.Factories;
using HMV_Player.MVVM.ViewModels;

namespace HMV_Player.Services;

/*
 * logic based on https://github.com/blackspherefollower/NogasmChart
 */
public class NogasmAnalyzerService {
    private readonly NotificationContainerViewModel _notificationContainerViewModel;
    
    List<double> pressure = new List<double>();
    List<double> avgPressure = new List<double>();
    public double CurrentPressure;
    public double AveragePressure;

    private readonly Regex nogasmRegex = new Regex(@"^(-?\d+(\.\d+)?),(\d+(\.\d+)?),(\d+(\.\d+)?)$");

    private readonly Regex nogasmRegex2 =
        new Regex(@"^\d+ ?,\d+ ?,(-?\d+(\.\d+)?) ?,(\d+(\.\d+)?) ?,(\d+(\.\d+)?) ?,(-?\d+(\.\d+)?) ?$");

    private string _buffer = "";
    

    public NogasmAnalyzerService(NotificationContainerViewModel notificationContainerViewModel) {
        _notificationContainerViewModel = notificationContainerViewModel;
    }

    public bool IsTrackingPort { get; private set; } = false;

    private SerialPort port;

    public async Task StartTrackingPort(string portString) {
        IsTrackingPort = true;
        try {
            bool trackingValid = await ValidatePortAsync(portString);
            if (!trackingValid) {
                _notificationContainerViewModel.ShowNotification("Failed to Start Tracking Nogasm", $"Due to Invalid Port: {portString}", NotificationType.Info);
                return;
            }
            port = new SerialPort(portString, 115200);
            port.DataReceived += OnPortDataReceived;
            port.Open();
        }
        catch (Exception e) {
            StopTrackingPort();
            ErrorLogger.Log(e);
            _notificationContainerViewModel.ShowNotification("Failed to Start Tracking Nogasm", e.Message, NotificationType.Error);
        }
    }

    private void OnPortDataReceived(object sender, SerialDataReceivedEventArgs e) {
        _buffer += port.ReadExisting().Replace("\r", "");

        int off;
        while ((off = _buffer.IndexOf('\n')) != -1) {
            var line = _buffer[..off];
            _buffer = _buffer[(off + 1)..];

            if (TryParseNogasmLine(line, out var curr, out var avg)) {
                CurrentPressure = curr;
                AveragePressure = avg;

                pressure.Add(curr);
                avgPressure.Add(avg);

                Console.WriteLine(avg);
            }
            else {
                Console.WriteLine($"Invalid line: {line}");
            }
        }
    }

    public void StopTrackingPort() {
        IsTrackingPort = false;
        if (port?.IsOpen == true) {
            port.DataReceived -= OnPortDataReceived;
            port.Close();
        }
        
    }

    public async Task<bool> ValidatePortAsync(string portString, int timeoutMs = 1000) {
        try {
            var validationPort = new SerialPort(portString, 115200);
            var tcs = new TaskCompletionSource<bool>();
            string buffer = "";

            void Handler(object s, SerialDataReceivedEventArgs e) {
                try {
                    buffer += validationPort.ReadExisting().Replace("\r", "");

                    int off;
                    while ((off = buffer.IndexOf('\n')) != -1) {
                        string line = buffer[..off];
                        buffer = buffer[(off + 1)..];

                        if (TryParseNogasmLine(line, out _, out _)) {
                            tcs.TrySetResult(true);
                            return;
                        }
                    }
                }
                catch {
                    // Invalid serial noise shouldn't crash validation
                }
            }

            validationPort.DataReceived += Handler;
            validationPort.Open();

            using var cts = new CancellationTokenSource(timeoutMs);
            cts.Token.Register(() => tcs.TrySetResult(false));

            bool result = await tcs.Task.ConfigureAwait(false);

            validationPort.DataReceived -= Handler;

            if (validationPort.IsOpen)
                validationPort.Close();

            return result;
        }
        catch {
            return false;
        }
    }


    public string[] ReadPorts() {
        string[] portNames = SerialPort.GetPortNames();
        return portNames;
    }

    private bool TryParseNogasmLine(string line, out double current, out double average) {
        current = 0;
        average = 0;

        var m = nogasmRegex.Match(line);
        if (!m.Success)
            m = nogasmRegex2.Match(line);

        if (!m.Success)
            return false;

        current = Convert.ToDouble(m.Groups[3].Value, CultureInfo.InvariantCulture);
        average = Convert.ToDouble(m.Groups[5].Value, CultureInfo.InvariantCulture);

        return true;
    }
}