using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
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

    private Func<VideosLoadingOverlayViewModel> _videoLoadingOverlayViewModelFactory;

    public VideoManagerViewModel(VideoDataStorageService videoDataStorageService,
        IThumbnailExtractor thumbNailExtractorService, IDialogueService dialogueService) {
        _videoLoadingOverlayViewModelFactory = () => {
            OnPropertyChanged(nameof(IsVideoLoadingOverlayActive));
            return new VideosLoadingOverlayViewModel(videoDataStorageService, thumbNailExtractorService, () => {
                CurrentVideoLoadingOverlayViewModel = null;
                OnPropertyChanged(nameof(IsVideoLoadingOverlayActive));
            });
        };
        _dialogueService = dialogueService;
        _videoDataStorageService = videoDataStorageService;

        VidCards.Clear();
        VidCards.Add(new VidCardModel() {
            Title = "Some Video 1",
            ImageSrc = "avares://HMV Player/Assets/Images/Placeholders/bear.jpg"
        });
        VidCards.Add(new VidCardModel() {
            Title = "Some Video 2",
            ImageSrc = "avares://HMV Player/Assets/Images/Placeholders/bear.jpg"
        });
        VidCards.Add(new VidCardModel() {
            Title = "Some Video 3",
            ImageSrc = "avares://HMV Player/Assets/Images/Placeholders/bear.jpg"
        });
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
        CurrentVideoDetailsOverallViewModel = new VideoDetailsViewModel(card, CloseVideoDetails);
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
    public void RefreshVideos() {
        CurrentVideoLoadingOverlayViewModel = _videoLoadingOverlayViewModelFactory.Invoke();
        OnPropertyChanged(nameof(IsVideoLoadingOverlayActive));
    }
}