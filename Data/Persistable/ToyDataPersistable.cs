using System.Collections.Generic;

namespace HMV_Player.Data.Persistable;

public class ToyDataPersistable {
    public List<ToyData> ToyDatas { get; set; } = new();

    public ToyDataPersistable() {
        
    }

    public ToyDataPersistable(Dictionary<string, ToyScriptProcessor> toyDatas) {
        ToyDatas = new();
        foreach (var lovenseToyScriptProcessor in toyDatas) {
            ToyData data = new(lovenseToyScriptProcessor.Value);
            ToyDatas.Add(data);
        }
    }

    public Dictionary<string, ToyScriptProcessor> ToDictionary() {
        Dictionary<string, ToyScriptProcessor> dic = new();
        foreach (var toyData in ToyDatas) {
            dic.Add(toyData.ToyId, new ToyScriptProcessor(toyData));
        }

        return dic;
    }
}