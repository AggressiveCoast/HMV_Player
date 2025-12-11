using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace HMV_Player.Services.Devices.Lovense.Response;

public class LovenseGetToysResponse {
    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("data")]
    public LovenseGetToysData Data { get; set; } = new();

    [JsonPropertyName("type")]
    public string Type { get; set; } = string.Empty;
}

public class LovenseGetToysData {
    [JsonPropertyName("toys")] public string ToysJson { get; set; } = string.Empty; // JSON string

    [JsonPropertyName("platform")]
    public string Platform { get; set; } = string.Empty;

    [JsonPropertyName("appType")]
    public string AppType { get; set; } = string.Empty;

    [JsonIgnore]
    public Dictionary<string, LovenseToy> Toys
        => string.IsNullOrWhiteSpace(ToysJson)
            ? new Dictionary<string, LovenseToy>()
            : JsonSerializer.Deserialize<Dictionary<string, LovenseToy>>(ToysJson) 
              ?? new Dictionary<string, LovenseToy>();
}

public class LovenseToy : IEquatable<LovenseToy>
{
    [JsonPropertyName("id")]
    public string Id { get; set; } = string.Empty;

    [JsonPropertyName("status")]
    public int Status { get; set; } = -1;

    [JsonPropertyName("version")]
    public string Version { get; set; } = string.Empty;

    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    [JsonPropertyName("battery")]
    public int Battery { get; set; }

    [JsonPropertyName("nickName")]
    public string NickName { get; set; } = string.Empty;

    [JsonPropertyName("shortFunctionNames")]
    public List<string> ShortFunctionNames { get; set; } = new();

    [JsonPropertyName("fullFunctionsNames")]
    public List<string> FullFunctionNames { get; set; } = new();

    public override string ToString() {
        if (string.IsNullOrEmpty(NickName)) {
            return Name;
        }
        
        return $"{Name} | {NickName}";
    }

    public bool Equals(LovenseToy? other) {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id == other.Id;
    }

    public override bool Equals(object? obj) {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((LovenseToy)obj);
    }

    public override int GetHashCode() {
        return Id.GetHashCode();
    }
}