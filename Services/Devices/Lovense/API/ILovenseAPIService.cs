using System.Threading.Tasks;
using HMV_Player.Services.Devices.Lovense.Request;
using HMV_Player.Services.Devices.Lovense.Response;

namespace HMV_Player.Services.Devices.Lovense.API;

public interface ILovenseApiService {

    public Task<bool> PostSetupPattern(LovenseSetupPatternRequest lovenseSetupPatternRequest);
    public Task<bool> PostPlayPattern(LovensePlayPatternRequest lovensePlayPatternRequest);
    public Task<bool> PostStopPattern(LovenseStopPatterRequest lovenseStopPatterRequest);
    
    public Task<bool> PostAction(LovenseFunctionRequest  lovenseFunctionRequest);
    
    public Task<LovenseGetToysResponse> GetToys();
    
    public Task<LovenseGetToyNameResponse> GetToyName();
    
    public Task<LovensePatternV2InitPlayResponse> PostPatternV2InitPlay(LovensePatternV2InitPlayRequest patternV2InitPlayRequest);
    public Task<LovensePatternV2PlayResponse> PostPatternV2Play(LovensePatternV2PlayRequest patternV2PlayRequest);
    public Task<LovensePatternV2StopResponse> PostPatternV2Stop(LovensePatternV2StopRequest lovensePatternV2StopRequest);
}