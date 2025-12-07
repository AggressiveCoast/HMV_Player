using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using HMV_Player.Data;
using HMV_Player.Data.Persistable;
using HMV_Player.MVVM.ViewModels.Base;
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
        IThumbnailExtractor thumbNailExtractorService, Action onVideosLoaded) {
        Task.Run(() => LoadVideos(onVideosLoaded));
        _thumbNailExtractorService = thumbNailExtractorService;
        _videoDataStorageService = videoDataStorageService;
    }

    private async Task LoadVideos(Action onVideosLoaded) {
        if (string.IsNullOrWhiteSpace(_videoDataStorageService.DataInstance.BaseLocation)) {
            onVideosLoaded?.Invoke();
            return;
        }
        
        _thumbNailExtractorService.ClearThumbnailCache();
        _videoDataStorageService.DataInstance.VideoFileDatas.Clear();
        
        string baseVideoPath = _videoDataStorageService.DataInstance.BaseLocation;

        var directories = Directory.GetDirectories(baseVideoPath, "*", SearchOption.AllDirectories);

        List<string> allFiles = new();
        foreach (var directory in directories) {
            var files = videoExtensions.SelectMany(ext =>
                Directory.EnumerateFiles(directory, ext, SearchOption.TopDirectoryOnly));
            allFiles.AddRange(files);
        }

        float fileCount = 0;
        foreach (var videoPath in allFiles) {
            fileCount++;
            string writePath = Path.Combine(HMVPlayerAppPaths.ThumbNailCache, Path.GetFileNameWithoutExtension(videoPath));
            CurrentFileBeingProcessed = writePath;
            var successfulWrite = await _thumbNailExtractorService.ExtractThumbnailAsync(videoPath, writePath);
            if (successfulWrite) {
                _videoDataStorageService.DataInstance.VideoFileDatas.Add(new VideoFileData() {
                    FullPath = videoPath,
                    Name = Path.GetFileNameWithoutExtension(videoPath),
                    ThumbnailPath =  writePath
                });
            }
            ProgressPercentage = fileCount / allFiles.Count;
        }

        _videoDataStorageService.Save();
        onVideosLoaded?.Invoke();
    }
}