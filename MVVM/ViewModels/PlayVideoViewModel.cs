using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HMV_Player.Data;
using HMV_Player.MVVM.ViewModels.Base;
using HMV_Player.MVVM.ViewModels.VideoPlayer;
using HMV_Player.Services.VideoPlayer;
using LibVLCSharp.Shared;

namespace HMV_Player.MVVM.ViewModels;

public partial class PlayVideoViewModel : PageViewModel {
    public override ApplicationPageName PageName => ApplicationPageName.PlayVideo;

    private readonly VideoPlayerViewModel _videoPlayerViewModel;
    

    public PlayVideoViewModel(VideoPlayerViewModel videoPlayerViewModel) {
        _videoPlayerViewModel = videoPlayerViewModel;
        _currentPageViewModel = _videoPlayerViewModel;
    }
    
    [ObservableProperty] private VideoPlayerViewModel  _currentPageViewModel;
    
    
}