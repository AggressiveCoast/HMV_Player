using System.ComponentModel;
using HMV_Player.Controls;
using HMV_Player.MVVM.ViewModels;

namespace HMV_Player.Services;

public class NotificationService {
    private static NotificationService _instance;
    
    private readonly NotificationContainerViewModel _viewModel;

    private NotificationService(NotificationContainerViewModel viewModel) {
        _viewModel = viewModel;
    }
    
    public static void Initialize(NotificationContainerViewModel viewModel) {
        _instance =  new NotificationService(viewModel);
    }
    
    public static void ShowNotification(string title, string text, NotificationType type = NotificationType.Info) {
        _instance._viewModel.ShowNotification(title, text, type);
    }
    
    
}