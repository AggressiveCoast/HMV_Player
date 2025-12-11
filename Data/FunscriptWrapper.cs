using System.Collections.Generic;

namespace HMV_Player.Data;

public class FunscriptWrapper {

    public FunscriptWrapper(FunscriptFile funscriptFile) {
        SourceFunscriptFile = funscriptFile;
    }

    public FunscriptFile SourceFunscriptFile;
}