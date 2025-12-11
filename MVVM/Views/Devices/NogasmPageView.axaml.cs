using System;
using System.ComponentModel;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using HMV_Player.Data.Persistable;
using HMV_Player.MVVM.ViewModels.Devices;
using HMV_Player.Services.Devices;
using HMV_Player.Services.Storage;
using HMV_Player.Services.Storage.Devices;

namespace HMV_Player.MVVM.Views.Devices;

public partial class NogasmPageView : UserControl {
    public NogasmPageView() {
        InitializeComponent();

        Unloaded += (sender, args) => {
            if (DataContext is NogasmPageViewModel vm) {
                vm.StopTestTracking();
            }
        };
        
        PressureThresholdTextBox.AddHandler(TextInputEvent, (sender, args) => { // no non numeric characters
            if (!char.IsDigit(args.Text[0])) {
                PressureThresholdTextBox.Text =
                    PressureThresholdTextBox.Text.Substring(0, PressureThresholdTextBox.Text.Length - 1);
                args.Handled = true; 
            }
            if (Int32.TryParse(PressureThresholdTextBox.Text, out var result) && result > EdgeToyInterceptorStorageService.MAX_PRESSURE_THRESHOLD) {
                PressureThresholdTextBox.Text = EdgeToyInterceptorStorageService.MAX_PRESSURE_THRESHOLD.ToString();
                args.Handled = true; 
            }

            if (string.IsNullOrWhiteSpace(PressureThresholdTextBox.Text)) {
                PressureThresholdTextBox.Text = "0";
                args.Handled = true; 
            }
        }, handledEventsToo: true);
    }

    private void PortComboBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e) {
        if (DataContext is NogasmPageViewModel vm) {
            vm.ValidatePortSection();
        }
    }
}