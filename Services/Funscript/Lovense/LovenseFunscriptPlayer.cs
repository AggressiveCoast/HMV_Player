using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HMV_Player.Data;
using HMV_Player.Data.Persistable;
using HMV_Player.Services.Devices;
using HMV_Player.Services.Devices.Controllers;
using HMV_Player.Services.Devices.Lovense.Response;
using HMV_Player.Utility;

namespace HMV_Player.Services.Funscript.Lovense;

public class LovenseFunscriptPlayer {
    private readonly ToyDataManager _toyDataManager;
    private readonly FunscriptWrapper _funscriptWrapper;
    private readonly LovenseDeviceController _lovenseController;
    private readonly EdgeToyInterceptorService _edgeToyInterceptorService;

    private bool _edgeInterceptorBlocking;
    private int _pingDelayInMilli;
    private string[] deviceIds;
    public bool _initializing;
    private FunScriptChannel _channel;
    private LovenseToysSettingsFile.LovenseDevice[] _applicableLovenseToys;

    private int _currentActionIndex = -1;

    private List<LovensePatternAction> _lovensePatternActions;
    private List<double> _scriptTimes;
    
    public LovenseFunscriptPlayer(FunScriptChannel channel, ToyDataManager toyDataManager,
        FunscriptWrapper funscriptWrapper,
        LovenseDeviceController lovenseController, EdgeToyInterceptorService edgeToyInterceptorService) {
        _toyDataManager = toyDataManager;
        _funscriptWrapper = funscriptWrapper;
        _lovenseController = lovenseController;
        _edgeToyInterceptorService = edgeToyInterceptorService;
        _channel = channel;
        _ = InitializeTask();
    }

    private async Task InitializeTask() {
        _initializing = true;
        var connectedToysResult = await _lovenseController.GetToys();
        LovenseToy[] currentlyConnectedToys = connectedToysResult.Data.Toys.Values.ToArray();
        _applicableLovenseToys = _toyDataManager.LovenseToySettingsStorageService.DataInstance.DevicesDict.Values
            .Where(dev =>
                dev.ApplicableChannel == _channel && dev.Enabled &&
                currentlyConnectedToys.Any(toy => toy.Id == dev.DeviceId)).ToArray();
        _pingDelayInMilli = await _lovenseController.FunctionPing(_applicableLovenseToys);
        _scriptTimes = _funscriptWrapper.SourceFunscriptFile.actions.Select(val => val.at).ToList();
        _lovensePatternActions = BuildLovensePattern(_funscriptWrapper);
        deviceIds = _applicableLovenseToys.Select(toy => toy.DeviceId).ToArray();
        //await _lovenseController.PostPatternV2InitPlay(deviceIds, lovensePatternActions, 0, _pingDelayInMilli);
        //await _lovenseController.PostPatternV2Stop(deviceIds);
        _initializing = false;
    }

    public void ProcessFunScript(long playerTime, bool isPlaying) {
        if (_initializing) return;
        if (!_edgeInterceptorBlocking && _edgeToyInterceptorService.PressureThresholdReached(_channel)) {
            _ = HandleEdgeInterceptor();
            return;
        }

        playerTime += 500; // 500 ms offset for lag
        if (playerTime < _scriptTimes[0]) return;
        _ = CalculateStrengthAndPost(playerTime);
    }

    public void PauseDevices() {
        _ = _lovenseController.PostFunction(_applicableLovenseToys, 0, 1);
    }

    private async Task CalculateStrengthAndPost(long playerTime) {
        int index = CalculateCurrentActionIndex(playerTime);
        if (index == _currentActionIndex || index >= _scriptTimes.Count - 1) {
            return;
        }

        _currentActionIndex = index;
        FunscriptFile.Action currentAction = _funscriptWrapper.SourceFunscriptFile.actions[index];
        FunscriptFile.Action nextAction = _funscriptWrapper.SourceFunscriptFile.actions[index + 1];

        int strength = convertActionsToStrength(currentAction, nextAction);
        double time = ((nextAction.at - currentAction.at) / 1000);
        if (strength == 0) return;
        Console.WriteLine(time);
        _ = _lovenseController.PostFunction(_applicableLovenseToys, strength, time);
    }


    private async Task HandleEndReached() {
        await _lovenseController.PostFunction(_applicableLovenseToys, 0, 1);
    }

    private async Task HandleEdgeInterceptor() {
        _ = RestoreEdgeInterceptorBlocking();
        await _lovenseController.PostFunction(_applicableLovenseToys, 0,
            _toyDataManager.EdgeToyInterceptorStorageService.DataInstance.nogasmData.NumberOfSecondsToStopBlocking);
    }

    private async Task RestoreEdgeInterceptorBlocking() {
        _edgeInterceptorBlocking = true;
        await Task.Delay(_toyDataManager.EdgeToyInterceptorStorageService.DataInstance.nogasmData
            .NumberOfSecondsToStopBlocking);
        _edgeInterceptorBlocking = false;
    }

    // recalculate pattern so it logically works with non stroke actions
    private List<LovensePatternAction> BuildLovensePattern(FunscriptWrapper funscriptWrapper) {
        List<LovensePatternAction> pattern = new List<LovensePatternAction>();
        foreach (var action in funscriptWrapper.SourceFunscriptFile.actions) {
            pattern.Add(new LovensePatternAction() {
                ts = (int)action.at,
                pos = action.pos,
            });
        }

        return pattern;
    }

    private int CalculateCurrentActionIndex(long playerTimeMilli) {
        int index = _scriptTimes.BinarySearch(playerTimeMilli);
        if (index < 0) {
            index = ~index;
        }

        return index;
    }

    private int convertActionsToStrength(FunscriptFile.Action currentAction, FunscriptFile.Action nextAction) {
        int positionDiff = Math.Abs(nextAction.pos - currentAction.pos);
        double timeDiffInSeconds = (nextAction.at - currentAction.at) / 1000;

        double velocity = positionDiff / timeDiffInSeconds;

        double maxVel = 200; //100 position difference / .5f timediff
        double minVel = 5; // 10 position diff / .5f timediff

        int strength;
        if (velocity < minVel) {
            strength = 0;
        }
        else if (velocity > maxVel) {
            strength = 100;
        }
        else {
            float alpha = MathExtensions.InverseLerp(minVel, maxVel, velocity);
            strength = (int)Math.Round(Single.Lerp(1, 100, alpha));
        }

        /*Console.WriteLine($"--- Action Conversion Report ---");
        Console.WriteLine($"Current Position (pos): {currentAction.pos}");
        Console.WriteLine($"Next Position (pos):    {nextAction.pos}");
        Console.WriteLine($"Position Difference:    {positionDiff}");
        Console.WriteLine($"-");
        Console.WriteLine($"Current Time (at):      {currentAction.at} ms");
        Console.WriteLine($"Next Time (at):         {nextAction.at} ms");
        Console.WriteLine($"Time Difference:        {timeDiffInSeconds:F3} s");
        Console.WriteLine($"-");
        Console.WriteLine($"Calculated Velocity:    {velocity:F3}");
        Console.WriteLine($"Min/Max Velocity Range: [{minVel} to {maxVel}]");*/
        return strength;
    }

    /*
     * strength should be normalized between 0 and 100
     */
    private async Task HandleLovense(FunscriptFile.Action currentAction, FunscriptFile.Action nextAction) {
        int positionDiff = Math.Abs(nextAction.pos - currentAction.pos);

        double seconds = (nextAction.at - currentAction.at) / 1000.0;
        if (seconds <= 0) {
            seconds = 0.001; // Minimum time step
        }

        double velocity = positionDiff / seconds;

        const double maxVelocity = 500.0; // Tune this value
        double normalizedVelocity = Math.Min(1.0, velocity / maxVelocity);

        int strength = (int)(normalizedVelocity * 100);

        strength = Math.Clamp(strength, 0, 100);
        Console.WriteLine(
            $"Action: [{currentAction.at}ms, pos:{currentAction.pos}] -> [{nextAction.at}ms, pos:{nextAction.pos}] | Velocity: {velocity:F2}, Strength: {strength}, Duration: {seconds:F3}s");
        await _lovenseController.PostFunction(_applicableLovenseToys, strength, seconds);
    }
}