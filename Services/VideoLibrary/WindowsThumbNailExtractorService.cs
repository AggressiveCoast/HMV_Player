using System;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;

namespace HMV_Player.Services.VideoLibrary;

public class WindowsThumbNailExtractorService : IThumbnailExtractor {
    public async Task<bool> ExtractThumbnailAsync(string videoPath, string outputPath) {
        return await Task.Run(() => {
            try {
                using (var capture = new VideoCapture(videoPath)) {
                    double durationMs = capture.Get(CapProp.FrameCount) / capture.Get(CapProp.Fps) * 1000;

                    // Clamp to 10%–90% of video duration
                    double startMs = durationMs * 0.10;
                    double endMs = durationMs * 0.90;

                    double randomMs = startMs + new Random().NextDouble() * (endMs - startMs);

                    capture.Set(CapProp.PosMsec, randomMs);

                    using var frame = new Mat();
                    bool success = capture.Read(frame);
                    if (!success || frame.IsEmpty) { // failed to read frame at time, default to first frame
                        capture.Set(CapProp.PosMsec, startMs);
                        capture.Read(frame);
                        if (frame.IsEmpty)
                            return false; // even the first frame failed, that's crazy
                    }

                    lowerImageRes(frame, outputPath);
                }

                return true;
            }
            catch (Exception e) {
                return false;
            }
        });
    }
    
    private void formatToSquareImage(Mat frame, string outputPath) {
        int squareSize = Math.Min(frame.Width, frame.Height);
        int x = (frame.Width - squareSize) / 2;
        int y = (frame.Height - squareSize) / 2;
        int desiredSize = 320;
        using var image = frame.ToImage<Bgr, byte>()
            .GetSubRect(new Rectangle(x, y, squareSize, squareSize))
            .Resize(desiredSize, desiredSize, Inter.Area, true);
        image.Save(outputPath);
    }

    private void lowerImageRes(Mat frame, string outputPath) {
        float ratio = (float) frame.Width / frame.Height;
        int desiredSize = 320;
        int width = desiredSize;
        int height = (int) (desiredSize / ratio);
        using var image = frame.ToImage<Bgr, byte>()
            .Resize(width, height, Inter.Area, true);
        image.Save(outputPath);
    }
}