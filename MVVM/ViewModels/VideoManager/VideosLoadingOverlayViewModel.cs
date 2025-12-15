using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using HMV_Player.Controls;
using HMV_Player.Data;
using HMV_Player.Data.Persistable;
using HMV_Player.MVVM.ViewModels.Base;
using HMV_Player.Services;
using HMV_Player.Services.Storage;
using HMV_Player.Services.VideoLibrary;

namespace HMV_Player.MVVM.ViewModels.VideoManager;

public partial class VideosLoadingOverlayViewModel : ViewModelBase {
    private readonly VideoDataStorageService _videoDataStorageService;
    private readonly IThumbnailExtractor _thumbNailExtractorService;

    string[] videoExtensions =
        ["*.mp4", "*.mkv", "*.avi", "*.mov", "*.wmv", "*.flv", "*.webm", "*.mpeg", "*.mpg", "*.m4v"];

    [ObservableProperty] private string _currentFileBeingProcessed;
    [ObservableProperty] private float _progressPercentage;

    public VideosLoadingOverlayViewModel(VideoDataStorageService videoDataStorageService,
        IThumbnailExtractor thumbNailExtractorService) {
        _thumbNailExtractorService = thumbNailExtractorService;
        _videoDataStorageService = videoDataStorageService;
    }

    public async Task RebuildVideoRegistry(Action onVideosLoaded) {
        if (string.IsNullOrWhiteSpace(_videoDataStorageService.DataInstance.BaseLocation)) {
            NotificationService.ShowNotification("Set the Base Directory", "", NotificationType.Info);
            onVideosLoaded?.Invoke();
            return;
        }

        _videoDataStorageService.DataInstance.VideoFileDatasDict.Clear();
        _thumbNailExtractorService.ClearThumbnailCache();


        string baseVideoPath = _videoDataStorageService.DataInstance.BaseLocation;

        List<string> directories = Directory.GetDirectories(baseVideoPath, "*", SearchOption.AllDirectories).ToList();
        directories.Add(baseVideoPath);

        List<string> allFiles = new();
        foreach (var directory in directories) {
            var files = videoExtensions.SelectMany(ext =>
                Directory.EnumerateFiles(directory, ext, SearchOption.TopDirectoryOnly));
            allFiles.AddRange(files);
        }

        float fileCount = 0;
        foreach (var videoPath in allFiles) {
            CurrentFileBeingProcessed = videoPath;
            fileCount++;
            string writePath = await GenerateThumbnail(videoPath);

            _videoDataStorageService.DataInstance.VideoFileDatasDict[videoPath] = new () {
                FullPath = videoPath,
                Name = Path.GetFileNameWithoutExtension(videoPath),
                ThumbnailPath = writePath
            };
            ProgressPercentage = fileCount / allFiles.Count;
        }

        _videoDataStorageService.Save();
        onVideosLoaded?.Invoke();
    }

    public async Task RefreshVideos(Action onVideosLoaded) {
        if (string.IsNullOrWhiteSpace(_videoDataStorageService.DataInstance.BaseLocation)) {
            NotificationService.ShowNotification("Set the Base Directory", "", NotificationType.Info);
            onVideosLoaded?.Invoke();
            return;
        }

        string baseVideoPath = _videoDataStorageService.DataInstance.BaseLocation;

        List<string> directories = Directory.GetDirectories(baseVideoPath, "*", SearchOption.AllDirectories).ToList();
        directories.Add(baseVideoPath);

        List<string> allFiles = new();
        foreach (var directory in directories) {
            var files = videoExtensions.SelectMany(ext =>
                Directory.EnumerateFiles(directory, ext, SearchOption.TopDirectoryOnly));
            allFiles.AddRange(files);
        }

        float fileCount = 0;
        foreach (var videoPath in allFiles) {
            CurrentFileBeingProcessed = videoPath;
            fileCount++;
            ProgressPercentage = fileCount / allFiles.Count;
            if (_videoDataStorageService.DataInstance.VideoFileDatasDict.Values.Any((file) => file.FullPath == videoPath && File.Exists(file.ThumbnailPath)))
                continue;

            string thumbnailLocation = await GenerateThumbnail(videoPath);

            var fileData = new VideoFileData() {
                FullPath = videoPath,
                Name = Path.GetFileNameWithoutExtension(videoPath),
            };

            if (_videoDataStorageService.DataInstance.VideoFileDatasDict.TryGetValue(videoPath,
                    out var videoFileData)) {
                fileData = videoFileData;
            }
            
            fileData.ThumbnailPath = thumbnailLocation;
            _videoDataStorageService.DataInstance.VideoFileDatasDict[videoPath] = fileData;

        }

        _videoDataStorageService.Save();
        onVideosLoaded?.Invoke();
    }

    private async Task<string> GenerateThumbnail(string videoPath) {
        string writePath = Path.Combine(HMVPlayerAppPaths.ThumbNailCache,
            Path.GetFileNameWithoutExtension(videoPath));
        writePath = Path.ChangeExtension(writePath, ".Jpeg");

        var successfulWrite = await _thumbNailExtractorService.ExtractThumbnailAsync(videoPath, writePath);
        if (successfulWrite) {
            return writePath;
        }

        return string.Empty;
    }

    public enum VideoLibraryBuildMode {
        FullRebuild,
        OnlyMissingFiles
    }
}