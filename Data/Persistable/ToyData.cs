namespace HMV_Player.Data.Persistable;

public class ToyData {
    public bool IsEnabled { get; set; }
    public string ToyId { get; set; }
    
    public DeviceBrandModelName DeviceBranchModelName { get; set; }

    public ToyData() {
        
    }
    public ToyData(ToyScriptProcessor toyScriptProcessor) {
        IsEnabled = toyScriptProcessor.IsEnabled;
        ToyId = toyScriptProcessor.ToyId;
        DeviceBranchModelName =  toyScriptProcessor.LovenseToyModelName;
    }
}