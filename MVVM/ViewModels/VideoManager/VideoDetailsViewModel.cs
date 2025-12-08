using System;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HMV_Player.MVVM.Models;
using HMV_Player.MVVM.ViewModels.Base;
using HMV_Player.Services.VideoPlayer;

namespace HMV_Player.MVVM.ViewModels.VideoManager;

public partial class VideoDetailsViewModel : ViewModelBase {
    private readonly Action? _onClose;
    private readonly IVideoPlayer _videoPlayerService;
    private MainViewModel _mainViewModel;
    
    private VidCardModel cardModel;
    public VideoDetailsViewModel(VidCardModel cardModel, MainViewModel mainViewModel, Action? onClose = null) {
        _onClose = onClose;
        this.cardModel = cardModel;
        ThumbnailImg = cardModel.ThumbnailImage;
        _mainViewModel = mainViewModel;
    }

    [ObservableProperty] private Bitmap? _thumbnailImg;

    [RelayCommand]
    private void CloseVideoDetails() {
        _onClose?.Invoke();
    }

    [RelayCommand]
    public void AssignFunscriptToChannel(int channel) {
        
    }

    [RelayCommand]
    public void LoadVideoAndOpenPlayPage() {
        _mainViewModel.LoadVideoAndGoToVideoPage(cardModel);
    }
    
}