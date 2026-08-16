using System.Drawing;

namespace ImageViewer.Application.Services
{
    public class ImageService
    {
        public Image LoadImage(string filePath)
        {
            // Image loading is isolated here so the form does not need to know
            // the details of converting a file path into a displayable Image.
            // Image.FromFile reads the image from disk and returns a disposable
            // Image object that the PictureBox can display.
            return Image.FromFile(filePath);
        }
    }
}
