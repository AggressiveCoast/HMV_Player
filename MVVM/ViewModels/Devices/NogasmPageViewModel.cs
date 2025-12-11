using System;
using System.Threading;
using System.Threading.Tasks;
using HMV_Player.Data;
using HMV_Player.Services;
using HMV_Player.Services.Storage;
using HMV_Player.Services.Storage.Devices;

namespace HMV_Player.MVVM.ViewModels.Devices;

public class NogasmPageViewModel : DevicesPageViewModel {
    public override DeviceBrands DeviceBrand => DeviceBrands.Nogasm;

    private readonly NogasmAnalyzerService _nogasmAnalyzerService;
    private readonly EdgeToyInterceptorStorageService _edgeToyInterceptorStorageService;

    public NogasmPageViewModel(NogasmAnalyzerService nogasmAnalyzerService, EdgeToyInterceptorStorageService edgeToyInterceptorStorageService) {
        _nogasmAnalyzerService = nogasmAnalyzerService;
        _edgeToyInterceptorStorageService = edgeToyInterceptorStorageService;
        IsInterceptChannel1On = _edgeToyInterceptorStorageService.DataInstance.nogasmData.InterceptChannel1;
        IsInterceptChannel2On = _edgeToyInterceptorStorageService.DataInstance.nogasmData.InterceptChannel2;
        IsInterceptChannel3On = _edgeToyInterceptorStorageService.DataInstance.nogasmData.InterceptChannel3;
        
        ValidatePortSection();
    }

    public string[] AvailablePorts => _nogasmAnalyzerService.ReadPorts();


    public string CurrentNogasmReading { get; private set; } = "Current Pressure Reading: (not reading)";
    private CancellationTokenSource? _ctsCurrentReading;
    public string SelectedPort {
        get => _edgeToyInterceptorStorageService.DataInstance.nogasmData.NogasmPort;
        set {
            _edgeToyInterceptorStorageService.DataInstance.nogasmData.NogasmPort = value;
            _edgeToyInterceptorStorageService.Save();
        }
    }
    
    public bool IsNogasmTrackingEnabled {
        get => _edgeToyInterceptorStorageService.DataInstance.nogasmData.IsNogasmTrackingEnabled;
        set {
            _edgeToyInterceptorStorageService.DataInstance.nogasmData.IsNogasmTrackingEnabled = value;
            _edgeToyInterceptorStorageService.Save();
        }
    }

    public bool IsInterceptChannel1On {
        get => _edgeToyInterceptorStorageService.DataInstance.nogasmData.InterceptChannel1;
        set {
            _edgeToyInterceptorStorageService.DataInstance.nogasmData.InterceptChannel1 = value;
            _edgeToyInterceptorStorageService.Save();
        }
    }
    
    public bool IsInterceptChannel2On {
        get => _edgeToyInterceptorStorageService.DataInstance.nogasmData.InterceptChannel2;
        set {
            _edgeToyInterceptorStorageService.DataInstance.nogasmData.InterceptChannel2 = value;
            _edgeToyInterceptorStorageService.Save();
        }
    }
    
    public bool IsInterceptChannel3On {
        get => _edgeToyInterceptorStorageService.DataInstance.nogasmData.InterceptChannel3;
        set {
            _edgeToyInterceptorStorageService.DataInstance.nogasmData.InterceptChannel3 = value;
            _edgeToyInterceptorStorageService.Save();
        }
    }

    public string NogasmPressureThreshold {
        get => _edgeToyInterceptorStorageService.DataInstance.nogasmData.OtherDevicePausePressureThreshold.ToString();
        set {
            string val = value;
            if (string.IsNullOrWhiteSpace(val)) {
                val = "0";
            }

            if (!char.IsAsciiDigit(val[^1])) {
                val = val.Substring(0, val.Length - 1);
            }
            
            _edgeToyInterceptorStorageService.DataInstance.nogasmData.OtherDevicePausePressureThreshold = Int32.Parse(val);
            _edgeToyInterceptorStorageService.Save();
        }
    }

    public bool IsPortValid { get; set; }

    public async Task ValidatePortSection() {
        if (IsNogasmTrackingEnabled) {
            return;
        }
        IsPortValid = await _nogasmAnalyzerService.ValidatePortAsync(SelectedPort, 1000); 
        OnPropertyChanged(nameof(IsPortValid));

        StopTestTracking();
        if (IsPortValid) {
            _nogasmAnalyzerService.StartTrackingPort(SelectedPort);
            startTestTracking();
        }
    }

    private async Task reportNogasmReading() {
        _ctsCurrentReading = new();
        var token = _ctsCurrentReading.Token;

        try {
            while (!token.IsCancellationRequested) {
                double currentPressureReading = _nogasmAnalyzerService.CurrentPressure;

                CurrentNogasmReading = "Current Pressure Reading: " + currentPressureReading;
                OnPropertyChanged(nameof(CurrentNogasmReading));
                await Task.Delay(TimeSpan.FromSeconds(0.3), token);
            }
        }
        catch (TaskCanceledException) {
            // this is fine
        }
    }
    
    private void startTestTracking()
    {
        _ = Task.Run(async () =>
        {
            try
            {
                await reportNogasmReading();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in reading nogasm: {ex}");
            }
        });
    }

    public void StopTestTracking() {
        CurrentNogasmReading = "Current Pressure Reading: (not reading)";
        OnPropertyChanged(nameof(CurrentNogasmReading));
        _nogasmAnalyzerService.StopTrackingPort();
        _ctsCurrentReading?.Cancel();
    }

}