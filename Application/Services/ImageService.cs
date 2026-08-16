using System.Drawing;
using ImageViewer.Domain.Processing;

namespace ImageViewer.Application.Services
{
    public class ImageService
    {
        private readonly GrayscaleProcessor grayscaleProcessor;

        public ImageService()
        {
            grayscaleProcessor = new GrayscaleProcessor();
        }

        public Bitmap LoadImage(string filePath)
        {
            // Image loading is isolated here so the form does not need to know
            // the details of converting a file path into a displayable Bitmap.
            // The temporary image is cloned so the file is not kept locked after
            // loading; the returned Bitmap must still be disposed by the caller.
            using Bitmap loadedImage = new Bitmap(filePath);

            return new Bitmap(loadedImage);
        }

        public Bitmap ApplyGrayscale(Bitmap image)
        {
            // The Application layer exposes the operation to the UI while the
            // pixel-by-pixel algorithm remains in the Domain layer.
            return grayscaleProcessor.Apply(image);
        }
    }
}
