using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO.Ports;
using System.Text.RegularExpressions;
using Avalonia.Controls;
using HMV_Player.Factories;

namespace HMV_Player.Services;

/*
 * logic based on https://github.com/blackspherefollower/NogasmChart
 */
public class NogasmAnalyzerService {
    
    List<double> pressure = new List<double>();
    List<double> avgPressure = new List<double>();
    public double CurrentPressure;
    public double AveragePressure;
    
    private readonly Regex nogasmRegex = new Regex(@"^(-?\d+(\.\d+)?),(\d+(\.\d+)?),(\d+(\.\d+)?)$");
    private readonly Regex nogasmRegex2 = new Regex(@"^\d+ ?,\d+ ?,(-?\d+(\.\d+)?) ?,(\d+(\.\d+)?) ?,(\d+(\.\d+)?) ?,(-?\d+(\.\d+)?) ?$");

    private string _buffer = "";

    public string CurrentTrackingPort;


    public NogasmAnalyzerService() {
        //StartTrackingPort();
    }

    public bool IsTrackingPort { get; private set; } = false;

    private SerialPort port;
    public void StartTrackingPort() {
        IsTrackingPort = true;
        string[] portNames = SerialPort.GetPortNames();
        string portNam = "COM3";
        foreach (var portName in portNames) {
            
        }
        
        //TODO: setup COM selection
        try {
            port = new SerialPort((string)portNam, 115200);
            port.DataReceived += OnPortDataReceived;
            port.Open();

        }
        catch (Exception e) {
            throw new NotImplementedException();
        }
        
    }

    private void OnPortDataReceived(object sender, SerialDataReceivedEventArgs e) {
        
        _buffer += port.ReadExisting().Replace("\r", "");
        var off = 0;
        while ((off = _buffer.IndexOf('\n')) != -1)
        {
            var line = _buffer.Substring(0, off);
            _buffer = _buffer.Substring(off+1);
            
            var m = nogasmRegex.Match(line);
            if (!m.Success)
                m = nogasmRegex2.Match(line);
            if (m.Success)
            {
                CurrentPressure = Convert.ToDouble(m.Groups[3].Value, new NumberFormatInfo());
                AveragePressure = Convert.ToDouble(m.Groups[5].Value, new NumberFormatInfo());
                pressure.Add(CurrentPressure);
                avgPressure.Add(AveragePressure);
                Console.WriteLine(AveragePressure);
            }
        }
    }

    public void StopTrackingPort() {
        IsTrackingPort = false;
        port.DataReceived -= OnPortDataReceived;
        port.Close();
    }
}