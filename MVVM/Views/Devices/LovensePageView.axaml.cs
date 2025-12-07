using Avalonia.Controls;
using HMV_Player.MVVM.ViewModels.Devices;
using HMV_Player.Services.Devices;

namespace HMV_Player.MVVM.Views.Devices;

public partial class LovensePageView : UserControl {
    public LovensePageView() {
        InitializeComponent();

        Loaded += (sender, args) => {
            if (DataContext is LovensePageViewModel viewModel) {
                viewModel.OnPageLoaded();
            }
        };

        Unloaded += (sender, args) => {
            if (DataContext is LovensePageViewModel viewModel) {
                viewModel.OnPageUnloaded();
            }
        };
    }
}