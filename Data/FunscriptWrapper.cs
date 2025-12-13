using System;
using System.Collections.Generic;
using System.Linq;

namespace HMV_Player.Data;

public class FunscriptWrapper {
    public FunscriptWrapper(FunscriptFile funscriptFile) {
        SourceFunscriptFile = funscriptFile;
        _actionTimeStamps = SourceFunscriptFile.actions.Select(val => val.at).ToList();
    }

    private readonly List<double> _actionTimeStamps; // milli

    public FunscriptFile SourceFunscriptFile;

    public int GetCurrentIndexFromTime(double milli) {
        int index = _actionTimeStamps.BinarySearch(milli);
        if (index < 0) { // extact match not found
            index = ~index;
        }

        return index;
    }

    public FunscriptFile.Action? GetActionAtIndex(int index) {
        if (index < 0 || index >= _actionTimeStamps.Count) return null;
        return SourceFunscriptFile.actions[index];
    }

    public int GetActionIndexAtMill(double milli) {
        int index = _actionTimeStamps.BinarySearch(milli);
        if (index < 0) { // extact match not found
            index = ~index;
        }

        return index;
    }
}