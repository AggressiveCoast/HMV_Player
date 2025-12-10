using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using Avalonia.Skia.Helpers;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HMV_Player.Controls.VideoManagement;
using HMV_Player.Data;
using HMV_Player.MVVM.Models;
using HMV_Player.MVVM.ViewModels.Base;
using HMV_Player.MVVM.ViewModels.VideoManager;
using HMV_Player.Services.DialogueWindow;
using HMV_Player.Services.Storage;
using HMV_Player.Services.VideoLibrary;

namespace HMV_Player.MVVM.ViewModels;

public partial class VideoManagerViewModel : PageViewModel {
    public override ApplicationPageName PageName => ApplicationPageName.VideoManager;

    private readonly VideoDataStorageService _videoDataStorageService;
    private readonly IDialogueService _dialogueService;
    private readonly MainViewModel _mainViewModel;

    private Func<VideosLoadingOverlayViewModel.VideoLibraryBuildMode, VideosLoadingOverlayViewModel> _videoLoadingOverlayViewModelFactory;

    public VideoManagerViewModel(VideoDataStorageService videoDataStorageService,
        IThumbnailExtractor thumbNailExtractorService, IDialogueService dialogueService, MainViewModel mainViewModel) {
        _mainViewModel = mainViewModel;
        _videoLoadingOverlayViewModelFactory = (videoLibraryBuildMode) => {
            OnPropertyChanged(nameof(IsVideoLoadingOverlayActive));
            return new VideosLoadingOverlayViewModel(videoDataStorageService, thumbNailExtractorService, () => {
                CurrentVideoLoadingOverlayViewModel = null;
                refreshVideoCards();
                OnPropertyChanged(nameof(IsVideoLoadingOverlayActive));
            }, videoLibraryBuildMode);
            
        };
        _dialogueService = dialogueService;
        _videoDataStorageService = videoDataStorageService;

        refreshVideoCards();
    }

    public ObservableCollection<VidCardModel> VidCards { get; set; } = new();

    [ObservableProperty] private VideoDetailsViewModel? _currentVideoDetailsOverallViewModel;
    [ObservableProperty] private VideosLoadingOverlayViewModel? _currentVideoLoadingOverlayViewModel;

    public string CurrentVideoBaseDirectoryDisplayString {
        get {
            string? rawPath = _videoDataStorageService.DataInstance.BaseLocation;

            string displayPath = string.IsNullOrWhiteSpace(rawPath) ? "(not set)" : rawPath;
            displayPath = displayPath.Replace(@"\\", "/");
            if (displayPath[^1] == '\\') displayPath = displayPath[..^1];

            string displayString = $"Base Directory: {displayPath}";
            return displayString;
        }
    }


    public bool IsVideoDetailsOverlayActive => CurrentVideoDetailsOverallViewModel != null;

    public bool IsVideoLoadingOverlayActive => _currentVideoLoadingOverlayViewModel != null;


    [RelayCommand]
    public void OpenVideoDetails(VidCardModel card) {
        Console.WriteLine($"Opening video: {card.Title}");
        CurrentVideoDetailsOverallViewModel = new VideoDetailsViewModel(_dialogueService, _videoDataStorageService, card, _mainViewModel, CloseVideoDetails);
        OnPropertyChanged(nameof(IsVideoDetailsOverlayActive));
    }

    private void CloseVideoDetails() {
        CurrentVideoDetailsOverallViewModel = null;
        OnPropertyChanged(nameof(IsVideoDetailsOverlayActive));
    }

    [RelayCommand]
    public async Task OnSelectBaseDirectoryClicked() {
        string? baseDir = await _dialogueService.OpenFolderSelectorAsync("Select Base Directory");
        if (baseDir == null) return;


        _videoDataStorageService.DataInstance.BaseLocation = baseDir;
        _videoDataStorageService.Save();

        OnPropertyChanged(nameof(CurrentVideoBaseDirectoryDisplayString));
    }

    [RelayCommand]
    public void RebuildVideoRegistry() {
        CurrentVideoLoadingOverlayViewModel = _videoLoadingOverlayViewModelFactory.Invoke(VideosLoadingOverlayViewModel.VideoLibraryBuildMode.FullRebuild);
        OnPropertyChanged(nameof(IsVideoLoadingOverlayActive));
    }

    [RelayCommand]
    public void RefreshVideoRegistry() {
        CurrentVideoLoadingOverlayViewModel = _videoLoadingOverlayViewModelFactory.Invoke(VideosLoadingOverlayViewModel.VideoLibraryBuildMode.OnlyMissingFiles);
        OnPropertyChanged(nameof(IsVideoLoadingOverlayActive));
    }

    private void refreshVideoCards() {
        ObservableCollection<VidCardModel> vidCards = new();
        foreach (var dataInstanceVideoFileData in _videoDataStorageService.DataInstance.VideoFileDatas) {
            string thumbNailPath = dataInstanceVideoFileData.ThumbnailPath;
            Bitmap thumbnail = null;
            try {
                thumbnail = new Bitmap(@thumbNailPath);
            }
            catch (Exception ex) {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
            }
            vidCards.Add(new VidCardModel() { // add to temp collection to avoid modified collection exception on actual ui collection
                Title =  dataInstanceVideoFileData.Name,
                ThumbnailImage = thumbnail,
                VideoPath = dataInstanceVideoFileData.FullPath
            });
        }

        VidCards = vidCards;
    }
}