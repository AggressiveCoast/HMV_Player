using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;

namespace HMV_Player.Services.DialogueWindow;

public class DialogueService(Func<TopLevel> _topLevel) : IDialogueService {
    public async Task<string?> OpenFolderSelectorAsync(string title = "Select Folder") {
        var folders = await _topLevel.Invoke().StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions() {
            Title = title,
            AllowMultiple = false
        });

        return folders.Count > 0 ? folders[0].Path.LocalPath : null;
    }

    public async Task<string?> OpenFileSelectorAsync(string title = "Select File") {
        var file = await _topLevel.Invoke().StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions() {
            Title = title,
            AllowMultiple = false,
            FileTypeFilter = new[] {
                new FilePickerFileType("Funscript") {
                    Patterns = new[] { "*.funscript" },
                    MimeTypes = new[] { "application/json" }
                }
            }
        });

        try {
            var fileEle = file.ElementAt(0);
            return fileEle?.Path.LocalPath;
        }
        catch {
            return null;
        }
    }
}