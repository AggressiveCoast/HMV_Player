using System.Threading.Tasks;
using HMV_Player.Services.Devices.Lovense.Response;

namespace HMV_Player.Services.Devices.Lovense.API;

public interface ILovenseApiService {

    public Task<bool> PostSetupPattern(LovenseSetupPatternRequest lovenseSetupPatternRequest);
    public Task<bool> PostPlayPattern(LovensePlayPatternRequest lovensePlayPatternRequest);
    public Task<bool> PostStopPattern(LovenseStopPatterRequest lovenseStopPatterRequest);
    
    public Task<LovenseGetToysResponse> GetToys();
}