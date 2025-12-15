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
        var response = await _httpClient.PostAsync(LovenseRemotePCDomain, content);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> PostPlayPattern(LovensePlayPatternRequest lovensePlayPatternRequest) {
        var json = JsonSerializer.Serialize(lovensePlayPatternRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(LovenseRemotePCDomain, content);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> PostStopPattern(LovenseStopPatterRequest lovenseStopPatterRequest) {
        var json = JsonSerializer.Serialize(lovenseStopPatterRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(LovenseRemotePCDomain, content);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> PostAction(LovenseFunctionRequest lovenseFunctionRequest) {
        var json = JsonSerializer.Serialize(lovenseFunctionRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(LovenseRemotePCDomain, content);
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

    public async Task<LovenseGetToyNameResponse> GetToyName() {
        var json = JsonSerializer.Serialize(new LovenseGetToyNameRequest());
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var httpResponse = await _httpClient.PostAsync(LovenseRemotePCDomain, content);
        
        var readAsStringAsync = await httpResponse.Content.ReadAsStringAsync();
        Console.WriteLine(readAsStringAsync);
        var response = await httpResponse.Content.ReadFromJsonAsync<LovenseGetToyNameResponse>();
        return response!;
    }

    public async Task<LovensePatternV2InitPlayResponse> PostPatternV2InitPlay(LovensePatternV2InitPlayRequest patternV2InitPlayRequest) {
        var json = JsonSerializer.Serialize(patternV2InitPlayRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var httpResponse = await _httpClient.PostAsync(LovenseRemotePCDomain, content);
        
        var readAsStringAsync = await httpResponse.Content.ReadAsStringAsync();
        Console.WriteLine(readAsStringAsync);
        var response = await httpResponse.Content.ReadFromJsonAsync<LovensePatternV2InitPlayResponse>();
        return response!;
    }

    public async Task<LovensePatternV2PlayResponse> PostPatternV2Play(LovensePatternV2PlayRequest patternV2PlayRequest) {
        var json = JsonSerializer.Serialize(patternV2PlayRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var httpResponse = await _httpClient.PostAsync(LovenseRemotePCDomain, content);
        
        var readAsStringAsync = await httpResponse.Content.ReadAsStringAsync();
        Console.WriteLine(readAsStringAsync);
        var response = await httpResponse.Content.ReadFromJsonAsync<LovensePatternV2PlayResponse>();
        return response!;
    }

    public async Task<LovensePatternV2StopResponse> PostPatternV2Stop(LovensePatternV2StopRequest lovensePatternV2StopRequest) {
        var json = JsonSerializer.Serialize(lovensePatternV2StopRequest);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var httpResponse = await _httpClient.PostAsync(LovenseRemotePCDomain, content);
        
        var readAsStringAsync = await httpResponse.Content.ReadAsStringAsync();
        Console.WriteLine(readAsStringAsync);
        var response = await httpResponse.Content.ReadFromJsonAsync<LovensePatternV2StopResponse>();
        return response!;
    }
}