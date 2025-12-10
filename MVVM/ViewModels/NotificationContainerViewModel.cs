using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using HMV_Player.Controls;
using HMV_Player.MVVM.Models;

namespace HMV_Player.MVVM.ViewModels;

public class NotificationContainerViewModel : INotifyPropertyChanged 
{
    private ObservableCollection<NotificationBubbleModel> _notifications = new();
    private int _notificationCount;

    public ObservableCollection<NotificationBubbleModel> Notifications => _notifications;

    public int NotificationCount 
    { 
        get => _notificationCount;
        set
        {
            _notificationCount = value;
            OnPropertyChanged();
        }
    }

    public ICommand RemoveNotificationCommand { get; }
        
    public NotificationContainerViewModel()
    {
        RemoveNotificationCommand = new RelayCommand<NotificationBubbleModel>(RemoveNotification);
        
        // Subscribe to collection changes to update count
        _notifications.CollectionChanged += (s, e) =>
        {
            NotificationCount = _notifications.Count;
        };
        
        // Test notification
        ShowNotification("Test", "This is a test notification!", NotificationType.Success, TimeSpan.FromSeconds(10));
    }
        
    public void ShowNotification(string title, string message, NotificationType type = NotificationType.Info, TimeSpan? autoCloseDuration = null)
    {
        System.Diagnostics.Debug.WriteLine($"ShowNotification: {title} - {message}");
        
        var notification = new NotificationBubbleModel
        {
            Title = title,
            Message = message,
            Type = type
        };
            
        Notifications.Add(notification);
        System.Diagnostics.Debug.WriteLine($"Notification added. Total count: {Notifications.Count}");
            
        if (autoCloseDuration.HasValue)
        {
            Task.Delay(autoCloseDuration.Value).ContinueWith(_ =>
            {
                Avalonia.Threading.Dispatcher.UIThread.Post(() => RemoveNotification(notification));
            });
        }
    }
        
    private void RemoveNotification(NotificationBubbleModel notification)
    {
        if (notification != null)
        {
            Notifications.Remove(notification);
            System.Diagnostics.Debug.WriteLine($"Notification removed. Total count: {Notifications.Count}");
        }
    }
        
    public event PropertyChangedEventHandler? PropertyChanged;
        
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}