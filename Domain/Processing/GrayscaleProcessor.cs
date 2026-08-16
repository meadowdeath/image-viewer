using System.Drawing;

namespace ImageViewer.Domain.Processing
{
    public class GrayscaleProcessor
    {
        public Bitmap Apply(Bitmap original)
        {
            // The original bitmap is left unchanged; filters should create a
            // separate result so the UI can restore the original image later.
            Bitmap result = new Bitmap(original.Width, original.Height);

            // Every pixel has an x and y coordinate, so the nested loops visit
            // each location in the bitmap exactly once.
            for (int y = 0; y < original.Height; y++)
            {
                for (int x = 0; x < original.Width; x++)
                {
                    // GetPixel returns the Color stored at this coordinate.
                    Color pixel = original.GetPixel(x, y);

                    int red = pixel.R;
                    int green = pixel.G;
                    int blue = pixel.B;

                    // The weighted formula approximates perceived luminance;
                    // human vision is more sensitive to green than to blue.
                    int gray = (int)(
                        0.299 * red +
                        0.587 * green +
                        0.114 * blue
                    );

                    // A grayscale pixel uses the same intensity for R, G, and B.
                    // The alpha channel is preserved so transparency is not lost.
                    Color newPixel = Color.FromArgb(
                        pixel.A,
                        gray,
                        gray,
                        gray
                    );

                    result.SetPixel(x, y, newPixel);
                }
            }

            return result;
        }
    }
}
