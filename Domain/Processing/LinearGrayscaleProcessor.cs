using System.Drawing;

namespace ImageViewer.Domain.Processing
{
    public class LinearGrayscaleProcessor
    {
        public Bitmap Apply(Bitmap original)
        {
            // This processor also creates a new bitmap so the original image
            // remains available for restore and comparison.
            Bitmap result = new Bitmap(original.Width, original.Height);

            for (int y = 0; y < original.Height; y++)
            {
                for (int x = 0; x < original.Width; x++)
                {
                    Color pixel = original.GetPixel(x, y);

                    double redLinear = StandardToLinear(pixel.R / 255.0);
                    double greenLinear = StandardToLinear(pixel.G / 255.0);
                    double blueLinear = StandardToLinear(pixel.B / 255.0);

                    // Linear-light luminance uses Rec. 709 / sRGB weights.
                    // The calculation happens after removing sRGB gamma.
                    double linearLuminance =
                        0.2126 * redLinear +
                        0.7152 * greenLinear +
                        0.0722 * blueLinear;

                    double standardGray = LinearToStandard(linearLuminance);
                    int gray = ClampToByte((int)Math.Round(standardGray * 255.0));

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

        private static double StandardToLinear(double standardChannel)
        {
            if (standardChannel <= 0.04045)
                return standardChannel / 12.92;

            return Math.Pow((standardChannel + 0.055) / 1.055, 2.4);
        }

        private static double LinearToStandard(double linearChannel)
        {
            if (linearChannel <= 0.0031308)
                return 12.92 * linearChannel;

            return 1.055 * Math.Pow(linearChannel, 1.0 / 2.4) - 0.055;
        }

        private static int ClampToByte(int value)
        {
            if (value < 0)
                return 0;

            if (value > 255)
                return 255;

            return value;
        }
    }
}
