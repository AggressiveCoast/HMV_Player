using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using HMV_Player.Services.Devices.Lovense.Request;
using HMV_Player.Services.Devices.Lovense.Response;

namespace HMV_Player.Services.Devices.Lovense.API;

public class LovenseApiService : ILovenseApiService {
    private const string LovenseRemotePCDomain = $"https://127-0-0-1.lovense.club:30010/command";

    private readonly HttpClient _httpClient;

    public LovenseApiService() {
        var handler = new HttpClientHandler();

        // **DANGEROUS: For local development only**
        // This callback causes the HttpClient to ignore any SSL certificate errors,
        // including self-signed or time-invalid certificates.
        handler.ServerCertificateCustomValidationCallback = 
            (message, cert, chain, sslPolicyErrors) => true;
        
        _httpClient = new HttpClient(handler);

    }
    public async Task<bool> PostSetupPattern(LovenseSetupPatternRequest lovenseSetupPatternRequest) {
        var json = JsonSerializer.Serialize(lovenseSetupPatternRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("/command", content);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> PostPlayPattern(LovensePlayPatternRequest lovensePlayPatternRequest) {
        var json = JsonSerializer.Serialize(lovensePlayPatternRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("/command", content);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> PostStopPattern(LovenseStopPatterRequest lovenseStopPatterRequest) {
        var json = JsonSerializer.Serialize(lovenseStopPatterRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync("/command", content);
        return response.IsSuccessStatusCode;
    }

    public async Task<LovenseGetToysResponse> GetToys() {
        var json = JsonSerializer.Serialize(new LovenseGetToysRequest());
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var httpResponse = await _httpClient.PostAsync(LovenseRemotePCDomain, content);
        
        var readAsStringAsync = await httpResponse.Content.ReadAsStringAsync();
        Console.WriteLine(readAsStringAsync);
        var response = await httpResponse.Content.ReadFromJsonAsync<LovenseGetToysResponse>();
        return response!;
    }
}