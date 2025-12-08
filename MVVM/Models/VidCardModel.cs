using Avalonia.Controls;
using Avalonia.Media.Imaging;

namespace HMV_Player.MVVM.Models;

public class VidCardModel {
    public string Title { get; set; }
    public Bitmap? ThumbnailImage {get; set;}

    public string VideoPath;
}