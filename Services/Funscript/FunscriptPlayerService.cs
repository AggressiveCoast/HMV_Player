using System;
using System.IO;
using System.Text.Json;
using HMV_Player.Data;
using HMV_Player.Data.Persistable;

namespace HMV_Player.Services.Funscript;

public class FunscriptPlayerService {

    private ToyScriptWrapper Script1;
    private ToyScriptWrapper Script2;
    private ToyScriptWrapper Script3;

    public FunscriptPlayerService() {
        
    }

    public void LoadScripts(VideoFileData videoFileData) {
        FunscriptFile funscriptFile1 = ReadFunscript(videoFileData.FunscriptChannel1FileLocation);
        Script1 = new(funscriptFile1);
        
        FunscriptFile funscriptFile2 = ReadFunscript(videoFileData.FunscriptChannel2FileLocation);
        Script2 = new(funscriptFile2);
        
        FunscriptFile funscriptFile3 = ReadFunscript(videoFileData.FunscriptChannel3FileLocation);
        Script3 = new(funscriptFile3);
    }

    public void PauseScripts() {
        
    }

    public void PlayScripts() {
        
    }

    private FunscriptFile ReadFunscript(string path) {
        FunscriptFile videoFileData;
        try {
            var json = File.ReadAllText(path);
            videoFileData = JsonSerializer.Deserialize<FunscriptFile>(json);
        }
        catch (Exception e) {
            throw new NotImplementedException("Properly handle exceptions: " + e.Message);
        }

        return videoFileData;
    }
}