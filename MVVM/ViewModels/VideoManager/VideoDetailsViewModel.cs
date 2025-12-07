using System;
using CommunityToolkit.Mvvm.Input;
using HMV_Player.MVVM.Models;
using HMV_Player.MVVM.ViewModels.Base;

namespace HMV_Player.MVVM.ViewModels.VideoManager;

public partial class VideoDetailsViewModel : ViewModelBase {
    private readonly Action? _onClose;

    private VidCardModel cardModel;
    public VideoDetailsViewModel(VidCardModel cardModel, Action? onClose = null) {
        _onClose = onClose;
        this.cardModel = cardModel;
    }

    [RelayCommand]
    private void CloseVideoDetails() {
        _onClose?.Invoke();
    }
    
}