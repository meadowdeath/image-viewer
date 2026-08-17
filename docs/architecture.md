# Architecture

Image Viewer uses a small layered architecture. The goal is to keep Windows Forms interaction separate from image-processing logic while preserving a codebase that is still understandable for a course project.

This is not a full Clean Architecture implementation. It follows a simple dependency rule:

```text
Presentation -> Application -> Domain
```

## Dependency Direction

Rules:

- Domain must not depend on Presentation.
- Domain must not depend on Application.
- Domain must not reference Windows Forms controls or dialogs.
- Application may depend on Domain.
- Presentation may depend on Application.
- Presentation owns the Windows Forms UI.

The current dependency flow is:

```text
MainForm
    -> ImageFileDialog
    -> ImageService
        -> GrayscaleProcessor
```

## Layers

### Domain

The Domain layer contains image-processing algorithms.

Current folder:

```text
Domain/
`-- Processing/
    `-- GrayscaleProcessor.cs
```

`GrayscaleProcessor` receives a `Bitmap`, creates a new `Bitmap`, reads pixels from the source image, writes transformed pixels into the result, and returns the result.

The Domain layer should contain image-processing concepts such as:

- `Bitmap`
- `Color`
- Pixel coordinates
- RGB and alpha channels
- `GetPixel()`
- `SetPixel()`

The Domain layer must not depend on:

- `Form`
- `Button`
- `PictureBox`
- `MessageBox`
- `OpenFileDialog`
- Presentation classes

This keeps the grayscale algorithm independent from how the image is selected or displayed.

### Application

The Application layer coordinates operations that the UI needs.

Current folder:

```text
Application/
`-- Services/
    `-- ImageService.cs
```

`ImageService` currently:

- loads an image path into a usable `Bitmap`;
- clones the loaded image so the file does not stay locked by the loading operation;
- exposes `ApplyGrayscale(Bitmap image)` to Presentation;
- delegates the pixel transformation to `GrayscaleProcessor`.

The distinction is:

```text
Application coordinates.
Domain transforms.
```

Application should not own Windows Forms controls, and it should not turn into a generic filter framework before the project needs one.

### Presentation

The Presentation layer owns the Windows Forms interface.

Current folders:

```text
Presentation/
|-- Dialogs/
|   `-- ImageFileDialog.cs
`-- MainForm.cs
```

`MainForm` owns:

- the form window;
- layout containers;
- labels;
- buttons;
- the `PictureBox`;
- the read-only image path `TextBox`;
- event subscriptions;
- enabling and disabling filter controls;
- original and processed image state;
- assigning images to the `PictureBox`;
- disposing image resources when they are replaced or when the form closes.

`ImageFileDialog` wraps `OpenFileDialog`. This belongs in Presentation because it interacts directly with the user and the operating system's file picker.

The UI is created entirely through C# code. The repository does not use Windows Forms Designer files for this implementation.

### Program.cs

`Program.cs` is the application entry point.

It initializes the WinForms application configuration and starts `MainForm`:

```csharp
System.Windows.Forms.Application.Run(new MainForm());
```

The fully qualified `System.Windows.Forms.Application` name avoids confusion with the project's own `ImageViewer.Application` namespace.

`Program.cs` should remain small.

## Image State Ownership

`MainForm` currently owns two bitmap references:

- `originalImage`
- `processedImage`

The original image is preserved so filters do not destroy the source. The processed image represents the current generated result, such as grayscale.

When a new image is selected:

1. The current `PictureBox.Image` reference is cleared.
2. The previous processed bitmap is disposed.
3. The previous original bitmap is disposed.
4. The new original bitmap is stored.
5. The new original bitmap is displayed.
6. Filter buttons are enabled.

When grayscale is applied:

1. `MainForm` asks `ImageService` to apply grayscale.
2. `ImageService` delegates to `GrayscaleProcessor`.
3. The processor returns a new bitmap.
4. `MainForm` displays the new processed bitmap.
5. Any previous processed bitmap is disposed.

## Repository Folders

```text
image-viewer/
|-- Application/
|   `-- Services/
|       `-- ImageService.cs
|-- Domain/
|   `-- Processing/
|       `-- GrayscaleProcessor.cs
|-- Presentation/
|   |-- Dialogs/
|   |   `-- ImageFileDialog.cs
|   `-- MainForm.cs
|-- docs/
|   |-- architecture.md
|   |-- image-processing.md
|   `-- diagrams/
|       |-- grayscale-processing-flow.mmd
|       |-- image-loading-flow.mmd
|       `-- layered-architecture.mmd
|-- Program.cs
`-- ImageViewer.csproj
```

## Why This Structure Matters

The structure keeps pixel-processing logic out of the UI. That matters because future course practices can add additional transformations without turning `MainForm` into the place where every algorithm lives.

It also makes responsibilities easier to explain:

- Presentation decides what the user sees.
- Application exposes operations needed by the UI.
- Domain performs the image transformation.

## Diagrams

- [Layered architecture](diagrams/layered-architecture.mmd)
- [Image loading flow](diagrams/image-loading-flow.mmd)
- [Grayscale processing flow](diagrams/grayscale-processing-flow.mmd)
