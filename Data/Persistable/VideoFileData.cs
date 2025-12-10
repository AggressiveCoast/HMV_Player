using System;

namespace HMV_Player.Data.Persistable;

public class VideoFileData : IEquatable<VideoFileData> {
    public string FullPath { get; set; } = "";
    public string Name { get; set; } = "";

    public string ThumbnailPath { get; set; } = "";

    public string FunscriptChannel1FileLocation { get; set; } = "";
    public string FunscriptChannel2FileLocation { get; set; } = "";
    public string FunscriptChannel3FileLocation { get; set; } = "";

    public bool Equals(VideoFileData? other) {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return FullPath == other.FullPath;
    }

    public override bool Equals(object? obj) {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((VideoFileData)obj);
    }

    public override int GetHashCode() {
        return FullPath.GetHashCode();
    }
}