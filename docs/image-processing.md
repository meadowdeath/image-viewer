# Image Processing

## Purpose

This document describes the image-processing concepts currently used by Image Viewer. It is intended to grow as future course practices add more transformations.

At the current stage, the only implemented transformation is grayscale.

## Raster Image Model

A raster image can be understood as a grid of pixels.

Each pixel is addressed by a coordinate:

```text
Pixel(x, y)
```

In the current implementation, the image is represented as a `Bitmap`. A `Bitmap` has a width and a height:

```text
width = number of columns
height = number of rows
```

## Pixel Coordinates

The grayscale processor traverses the bitmap with nested loops:

```text
for y from 0 to Height - 1
    for x from 0 to Width - 1
        process Pixel(x, y)
```

The outer loop walks through rows. The inner loop walks through columns.

## Color Channels

Each pixel has color components:

```text
R = red
G = green
B = blue
A = alpha
```

The alpha channel represents transparency. The current grayscale implementation preserves it.

## Original vs. Processed Image

Filtering creates two conceptual image states:

```text
Original image
Processed image
```

The original image is the source selected from disk. It must remain unchanged so the user can return to it after applying a filter.

The processed image is a generated result, such as a grayscale copy.

Current behavior:

```text
Select image
    |
    v
Store original bitmap
    |
    v
Display original

Apply Grayscale
    |
    v
Create processed bitmap
    |
    v
Display processed bitmap

Original
    |
    v
Display preserved original again
```

Selecting a new image resets both states. The previous processed image and previous original image are disposed.

## Current Transformations

### Grayscale

Grayscale conversion is implemented in:

```text
Domain/Processing/GrayscaleProcessor.cs
```

#### Algorithm

For each pixel, the processor reads the original color, calculates a grayscale intensity, and writes a new color to the result bitmap.

The formula is:

```text
Gray = 0.299R + 0.587G + 0.114B
```

The result uses the same value for all RGB channels:

```text
R = Gray
G = Gray
B = Gray
```

#### Pixel Traversal

The implementation visits every coordinate in the source bitmap:

```csharp
for (int y = 0; y < original.Height; y++)
{
    for (int x = 0; x < original.Width; x++)
    {
        Color pixel = original.GetPixel(x, y);
        // transform pixel
        result.SetPixel(x, y, newPixel);
    }
}
```

This direct traversal is intentional. It makes the relationship between the image matrix and the code easy to see.

#### RGB Conversion

The implementation reads red, green, and blue separately:

```text
red = pixel.R
green = pixel.G
blue = pixel.B
```

The weighted formula gives green the strongest influence because human vision is more sensitive to green light. Blue has the smallest weight.

This produces a luminance-based grayscale value instead of a simple average.

#### Alpha Preservation

The processor preserves the original alpha channel:

```csharp
Color.FromArgb(pixel.A, gray, gray, gray)
```

This prevents transparent or partially transparent pixels from becoming fully opaque.

#### Complexity

For an image with:

```text
width = W
height = H
```

the algorithm visits:

```text
W x H
```

pixels.

The traversal complexity is:

```text
O(W x H)
```

`GetPixel()` and `SetPixel()` are clear but not optimized for large images. Faster approaches such as `Bitmap.LockBits` are not used because the current practice focuses on understanding direct pixel access.

## Processing Pipeline

Current grayscale pipeline:

```text
Original Bitmap
    |
    v
ImageService.ApplyGrayscale
    |
    v
GrayscaleProcessor.Apply
    |
    v
Processed Bitmap
    |
    v
PictureBox
```

The UI requests the operation. The Application layer exposes it. The Domain layer performs the transformation.

## Future Extensions

Future practices may add more transformations, such as:

- negative;
- thresholding;
- brightness;
- contrast;
- additional pixel transformations.

These are planned possibilities only. They are not implemented in the current repository.
