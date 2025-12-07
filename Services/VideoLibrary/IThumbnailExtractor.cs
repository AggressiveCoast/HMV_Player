using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using HMV_Player.Data;

namespace HMV_Player.Services.VideoLibrary;

public interface IThumbnailExtractor {

    public Task<bool> ExtractThumbnailAsync(string videoPath, string outputPath);
    
    public void ClearThumbnailCache() {
        List<string> directories =
            Directory.GetDirectories(HMVPlayerAppPaths.ThumbNailCache, "*", SearchOption.AllDirectories).ToList();
        directories.Add(HMVPlayerAppPaths.ThumbNailCache);
        foreach (var directory in directories) {
            foreach (var file in Directory.GetFiles(directory,  "*", SearchOption.TopDirectoryOnly)) {
                File.Delete(file);
            }
        }
        
        foreach (var directory in directories) {
            if (directory != HMVPlayerAppPaths.ThumbNailCache) {
                if (Directory.Exists(directory)) {
                    Directory.Delete(directory, true);
                }
            }
        }
        
    }
}