using CommunityToolkit.Mvvm.ComponentModel;
using HMV_Player.Data;
using HMV_Player.MVVM.ViewModels.Base;
using HMV_Player.MVVM.ViewModels.VideoPlayer;
using HMV_Player.Services.Devices;

namespace HMV_Player.MVVM.ViewModels;

public partial class PlayVideoViewModel : PageViewModel {
    public override ApplicationPageName PageName => ApplicationPageName.PlayVideo;
    private readonly ToyScriptPlayerService _toyScriptPlayerService;
    

    public PlayVideoViewModel(VideoPlayerViewModel videoPlayerViewModel, ToyScriptPlayerService toyScriptPlayerService) {
        _videoPlayerViewModel = videoPlayerViewModel;
        _toyScriptPlayerService = toyScriptPlayerService;
    }
    
    [ObservableProperty] private VideoPlayerViewModel  _videoPlayerViewModel;
    
    
    
    
}