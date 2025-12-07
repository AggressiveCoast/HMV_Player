using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HMV_Player.Data;
using HMV_Player.Factories;
using HMV_Player.MVVM.ViewModels.Base;
using HMV_Player.MVVM.Views;

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

    public MainViewModel(PageFactory pageFactory) {
        _pageFactory = pageFactory;
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
}