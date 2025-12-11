using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HMV_Player.Data;
using HMV_Player.Data.Persistable;
using HMV_Player.Factories;
using HMV_Player.MVVM.Models;
using HMV_Player.MVVM.ViewModels.Base;
using HMV_Player.MVVM.ViewModels.VideoPlayer;
using HMV_Player.MVVM.Views;
using HMV_Player.Services.Devices;
using HMV_Player.Services.VideoPlayer;
using LibVLCSharp.Shared;

namespace HMV_Player.MVVM.ViewModels;

public partial class MainViewModel : ViewModelBase {
    [ObservableProperty] private bool _sideMenuExpanded = true;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HomePageIsActive))]
    [NotifyPropertyChangedFor(nameof(PlayVideoPageIsActive))]
    [NotifyPropertyChangedFor(nameof(DevicesPageIsActive))]
    [NotifyPropertyChangedFor(nameof(VideoManagerIsActive))]
    private PageViewModel _currentPage;

    public bool HomePageIsActive => CurrentPage.PageName == ApplicationPageName.Home;
    public bool PlayVideoPageIsActive => CurrentPage.PageName == ApplicationPageName.PlayVideo;
    public bool DevicesPageIsActive => CurrentPage.PageName == ApplicationPageName.Devices;

    public bool VideoManagerIsActive => CurrentPage.PageName == ApplicationPageName.VideoManager;

    private readonly PageFactory _pageFactory;
    private readonly IVideoPlayer _videoPlayer;
    private readonly NotificationContainerViewModel _notificationContainerViewModel;
    private readonly VideoPlayerViewModel _videoPlayerViewModel;
    private readonly ToyScriptPlayerService _toyScriptPlayerService;
    public NotificationContainerViewModel NotificationContainerViewModel => _notificationContainerViewModel;

    public MainViewModel(PageFactory pageFactory, IVideoPlayer videoPlayer,
        NotificationContainerViewModel notificationContainerViewModel, VideoPlayerViewModel videoPlayerViewModel,
        ToyScriptPlayerService toyScriptPlayerService) {
        _notificationContainerViewModel = notificationContainerViewModel;
        _pageFactory = pageFactory;
        _videoPlayer = videoPlayer;
        _videoPlayerViewModel = videoPlayerViewModel;
        _toyScriptPlayerService = toyScriptPlayerService;
        GoToHome();
    }

    [RelayCommand]
    private void GoToHome() {
        CurrentPage = _pageFactory.GetPageViewModel(ApplicationPageName.Home);
    }

    [RelayCommand]
    private void GoToPlayVideo() {
        CurrentPage = _pageFactory.GetPageViewModel(ApplicationPageName.PlayVideo);
    }

    [RelayCommand]
    private void GoToDevices() {
        CurrentPage = _pageFactory.GetPageViewModel(ApplicationPageName.Devices);
    }

    [RelayCommand]
    private void GoToVideoManager() {
        CurrentPage = _pageFactory.GetPageViewModel(ApplicationPageName.VideoManager);
    }

    public void LoadVideoAndGoToVideoPage(VideoFileData videoFileData) {
        _toyScriptPlayerService.SetVideoFileData(videoFileData);
        _videoPlayer.LoadMedia(videoFileData.FullPath);
        _videoPlayerViewModel.SetVideoPlayerState(VideoPlayerViewModel.VideoPlayerState.MediaLoaded);
        CurrentPage = _pageFactory.GetPageViewModel(ApplicationPageName.PlayVideo);
    }
    
    
}