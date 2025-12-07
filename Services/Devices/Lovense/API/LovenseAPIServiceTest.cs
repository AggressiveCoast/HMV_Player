using System.Collections.Generic;
using System.Threading.Tasks;
using HMV_Player.Services.Devices.Lovense.Response;

namespace HMV_Player.Services.Devices.Lovense.API;

public class LovenseAPIServiceTest : ILovenseApiService {
    public Task<bool> PostSetupPattern(LovenseSetupPatternRequest lovenseSetupPatternRequest) {
        throw new System.NotImplementedException();
    }

    public Task<bool> PostPlayPattern(LovensePlayPatternRequest lovensePlayPatternRequest) {
        throw new System.NotImplementedException();
    }

    public Task<bool> PostStopPattern(LovenseStopPatterRequest lovenseStopPatterRequest) {
        throw new System.NotImplementedException();
    }

    public Task<LovenseGetToysResponse> GetToys() {
        var response = new LovenseGetToysResponse {
            Code = 200,
            Data = new LovenseGetToysData {
                AppType = "remote",
                Platform = "win",
                ToysJson =
                    "{\"0C2A6FE11153\":{\"id\":\"0C2A6FE11153\",\"nickName\":\"\",\"name\":\"Gush\",\"battery\":94,\"version\":\"2\",\"status\":1,\"shortFunctionNames\":[\"o\"],\"fullFunctionsNames\":[\"Oscillate\"]}}"
            },
            Type = "OK"
        };

        return Task.FromResult(response);
    }
}