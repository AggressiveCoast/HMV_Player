using System;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using HMV_Player.Data.Persistable;
using HMV_Player.MVVM.Models;
using HMV_Player.MVVM.ViewModels.Base;
using HMV_Player.Services.DialogueWindow;
using HMV_Player.Services.Storage;
using HMV_Player.Services.VideoPlayer;

namespace HMV_Player.MVVM.ViewModels.VideoManager;

public partial class VideoDetailsViewModel : ViewModelBase {
    private readonly Action? _onClose;
    private readonly IVideoPlayer _videoPlayerService;
    private readonly VideoDataStorageService _videoDataStorageService;
    private readonly IDialogueService _dialogueService;
    private MainViewModel _mainViewModel;

    private VidCardModel cardModel;
    private VideoFileData videoFileData;

    public VideoDetailsViewModel(IDialogueService dialogueService, VideoDataStorageService videoDataStorageService,
        VidCardModel cardModel, MainViewModel mainViewModel, Action? onClose = null) {
        _onClose = onClose;
        this.cardModel = cardModel;
        ThumbnailImg = cardModel.ThumbnailImage;
        _mainViewModel = mainViewModel;
        _videoDataStorageService = videoDataStorageService;
        _dialogueService = dialogueService;
        videoFileData = _videoDataStorageService.DataInstance.GetFileData(cardModel.VideoPath);
    }

    [ObservableProperty] private Bitmap? _thumbnailImg;

    public string FileLocation1 =>  videoFileData
        .FunscriptChannel1FileLocation;

    public string FileLocation2 => videoFileData
        .FunscriptChannel2FileLocation;

    public string FileLocation3 => videoFileData
        .FunscriptChannel3FileLocation;

    [RelayCommand]
    private void CloseVideoDetails() {
        _onClose?.Invoke();
    }

    [RelayCommand]
    public async Task AssignFunscriptToChannel(int channelIndex) {
        if (videoFileData == null) {
            throw new Exception("No video file data found for file path: " + cardModel.VideoPath);
        }

        string? file = await _dialogueService.OpenFileSelectorAsync("Select");

        if (string.IsNullOrEmpty(file)) {
            return;
        }
        switch (channelIndex) {
            case 1:
                videoFileData.FunscriptChannel1FileLocation = file;
                break;
            case 2:
                videoFileData.FunscriptChannel2FileLocation = file;
                break;
            case 3:
                videoFileData.FunscriptChannel3FileLocation = file;
                break;
        }

        RefreshPropsForChannel(channelIndex);

        _videoDataStorageService.Save();
    }

    [RelayCommand]
    public void ClearFile(int channelIndex) {
        if (videoFileData == null) {
            throw new Exception("No video file data found for file path: " + cardModel.VideoPath);
        }

        switch (channelIndex) {
            case 1:
                videoFileData.FunscriptChannel1FileLocation = string.Empty;
                break;
            case 2:
                videoFileData.FunscriptChannel2FileLocation = string.Empty;
                break;
            case 3:
                videoFileData.FunscriptChannel3FileLocation = string.Empty;
                break;
        }
        RefreshPropsForChannel(channelIndex);

        _videoDataStorageService.Save();
    }

    private void RefreshPropsForChannel(int channelIndex) {
        switch (channelIndex) {
            case 1:
                OnPropertyChanged(nameof(FileLocation1));
                break;
            case 2:
                OnPropertyChanged(nameof(FileLocation2));
                break;
            case 3:
                OnPropertyChanged(nameof(FileLocation3));
                break;
        }
    }

    [RelayCommand]
    public void LoadVideoAndOpenPlayPage() {
        _mainViewModel.LoadVideoAndGoToVideoPage(cardModel);
    }
}