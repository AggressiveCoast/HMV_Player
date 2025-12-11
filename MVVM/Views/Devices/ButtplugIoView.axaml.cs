using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using HMV_Player.MVVM.ViewModels.Devices;

namespace HMV_Player.MVVM.Views.Devices;

public partial class ButtplugIoView : UserControl {
    public ButtplugIoView() {
        InitializeComponent();
    }

    private void SelectingItemsControl_OnSelectionChanged(object? sender, SelectionChangedEventArgs e) {
        if (DataContext is ButtplugIoViewModel vm) {
            string? itemSelected = null;
            if (e.AddedItems.Count > 0) {
                itemSelected = e.AddedItems[0].ToString();
            }
            vm.OnDeviceChanged(itemSelected);
        }
    }
}