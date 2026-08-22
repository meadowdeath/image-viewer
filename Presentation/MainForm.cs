using ImageViewer.Application.Services;
using ImageViewer.Presentation.Dialogs;

namespace ImageViewer.Presentation
{
    public class MainForm : Form
    {
        private const int ButtonMinWidth = 120;
        private const int ButtonMinHeight = 40;

        private readonly Button btnOpenImage;
        private readonly Button btnOriginal;
        private readonly Button btnGrayscale;
        private readonly Button btnLinearGrayscale;
        private readonly PictureBox pictureBoxImage;
        private readonly TextBox txtImagePath;

        // Filtering needs separate image state: the original must stay
        // unchanged while processed images can be created and replaced.
        private Bitmap? originalImage;
        private Bitmap? processedImage;

        // The form coordinates the UI workflow, while these classes keep file
        // selection, image loading, and processing out of presentation code.
        private readonly ImageService imageService;
        private readonly ImageFileDialog imageFileDialog;

        public MainForm()
        {
            imageService = new ImageService();
            imageFileDialog = new ImageFileDialog();

            ConfigureForm();

            pictureBoxImage = CreatePictureBox();
            txtImagePath = CreatePathTextBox();
            btnOpenImage = CreateOpenImageButton();
            btnOriginal = CreateFilterButton("Original");
            btnGrayscale = CreateFilterButton("Grayscale");
            btnLinearGrayscale = CreateFilterButton("Linear Grayscale");
            // The buttons auto-size to their text, then share the largest
            // minimum size so the set stays uniform.
            ApplyUniformButtonMinimumSize(
                btnOpenImage,
                btnOriginal,
                btnGrayscale,
                btnLinearGrayscale
            );
            SetFilterButtonsEnabled(false);

            Controls.Add(CreateLayout());

            // Click events route user actions to methods that coordinate UI
            // state; the actual file loading and processing stay elsewhere.
            btnOpenImage.Click += BtnOpenImage_Click;
            btnOriginal.Click += BtnOriginal_Click;
            btnGrayscale.Click += BtnGrayscale_Click;
            btnLinearGrayscale.Click += BtnLinearGrayscale_Click;
        }

        private void ConfigureForm()
        {
            Text = "Image Viewer";
            MinimumSize = new Size(800, 520);
            Width = 1000;
            Height = 700;
            StartPosition = FormStartPosition.CenterScreen;
        }

        private Control CreateLayout()
        {
            // TableLayoutPanel lets the window resize by distributing space
            // through rows and columns instead of fixed pixel coordinates.
            TableLayoutPanel mainLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(16)
            };

            mainLayout.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100F)
            );

            mainLayout.RowStyles.Add(
                new RowStyle(SizeType.AutoSize)
            );

            // Percent rows receive the remaining available space, which keeps
            // the image area responsive as the form changes size.
            mainLayout.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100F)
            );

            // AutoSize rows keep labels and controls at their preferred height.
            mainLayout.RowStyles.Add(
                new RowStyle(SizeType.AutoSize)
            );

            Label titleLabel = CreateLabel(
                "Image Viewer",
                18F,
                FontStyle.Bold
            );

            titleLabel.Anchor = AnchorStyles.None;
            titleLabel.Margin = new Padding(0, 0, 0, 12);

            mainLayout.Controls.Add(titleLabel, 0, 0);
            mainLayout.Controls.Add(CreateContentLayout(), 0, 1);
            mainLayout.Controls.Add(CreateFileSection(), 0, 2);

            return mainLayout;
        }

        private Control CreateContentLayout()
        {
            TableLayoutPanel contentLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1
            };

            contentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 72F));
            contentLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 28F));
            contentLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

            contentLayout.Controls.Add(CreateImageSection(), 0, 0);
            contentLayout.Controls.Add(CreateFilterSection(), 1, 0);

            return contentLayout;
        }

        private Control CreateImageSection()
        {
            Panel imagePanel = CreatePanel();
            imagePanel.Padding = new Padding(12);
            imagePanel.Margin = new Padding(0, 0, 12, 0);
            imagePanel.Controls.Add(pictureBoxImage);

            return imagePanel;
        }

        private Control CreateFileSection()
        {
            // This section uses layout rules so the path field stretches with
            // the window while the button can remain centered in its own row.
            TableLayoutPanel fileLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                ColumnCount = 1,
                RowCount = 3,
                Margin = new Padding(0, 14, 0, 0)
            };

            fileLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            fileLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            fileLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            Label pathLabel = CreateLabel("Image path", 9F, FontStyle.Regular);
            pathLabel.Margin = new Padding(0, 0, 0, 6);

            txtImagePath.Margin = new Padding(0, 0, 0, 12);
            btnOpenImage.Margin = new Padding(0);
            // In a TableLayoutPanel cell, AnchorStyles.None centers the control
            // without manually calculating Left or Top coordinates.
            btnOpenImage.Anchor = AnchorStyles.None;

            fileLayout.Controls.Add(pathLabel, 0, 0);
            fileLayout.Controls.Add(txtImagePath, 0, 1);
            fileLayout.Controls.Add(btnOpenImage, 0, 2);

            return fileLayout;
        }

        private Control CreateFilterSection()
        {
            Panel filterPanel = CreatePanel();
            filterPanel.Padding = new Padding(14);
            filterPanel.Margin = new Padding(0);

            Label filterLabel = CreateLabel("Filters Section", 11F, FontStyle.Bold);
            filterLabel.Dock = DockStyle.Top;
            filterLabel.Height = 30;
            filterLabel.TextAlign = ContentAlignment.MiddleCenter;
            filterLabel.Margin = new Padding(0);

            // A one-column TableLayoutPanel centers each filter button through
            // AnchorStyles.None, without fixed Left or Top coordinates.
            TableLayoutPanel buttonLayout = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                AutoSize = true,
                ColumnCount = 1,
                RowCount = 3,
                Padding = new Padding(0, 24, 0, 0)
            };

            buttonLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            buttonLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            buttonLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            buttonLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

            buttonLayout.Controls.Add(btnOriginal);
            buttonLayout.Controls.Add(btnGrayscale);
            buttonLayout.Controls.Add(btnLinearGrayscale);

            filterPanel.Controls.Add(buttonLayout);
            filterPanel.Controls.Add(filterLabel);

            return filterPanel;
        }

        private PictureBox CreatePictureBox()
        {
            return new PictureBox
            {
                // DockStyle.Fill lets the layout container control the image
                // display size as the user resizes the window.
                Dock = DockStyle.Fill,
                // Zoom preserves the image proportions while fitting it inside
                // the available PictureBox area.
                SizeMode = PictureBoxSizeMode.Zoom,
                BorderStyle = BorderStyle.FixedSingle
            };
        }

        private TextBox CreatePathTextBox()
        {
            return new TextBox
            {
                Dock = DockStyle.Top,
                // ReadOnly keeps the actual selected path visible and
                // selectable, unlike disabling the control.
                ReadOnly = true,
                BorderStyle = BorderStyle.FixedSingle
            };
        }

        private Button CreateOpenImageButton()
        {
            return new Button
            {
                Text = "Select Image",
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                MinimumSize = new Size(ButtonMinWidth, ButtonMinHeight),
                Padding = new Padding(14, 6, 14, 6)
            };
        }

        private Button CreateFilterButton(string text)
        {
            return new Button
            {
                Text = text,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink,
                MinimumSize = new Size(ButtonMinWidth, ButtonMinHeight),
                Margin = new Padding(0, 0, 0, 14),
                Anchor = AnchorStyles.None
            };
        }

        private static void ApplyUniformButtonMinimumSize(params Button[] buttons)
        {
            int width = ButtonMinWidth;
            int height = ButtonMinHeight;

            foreach (Button button in buttons)
            {
                Size preferredSize = button.GetPreferredSize(Size.Empty);
                width = Math.Max(width, preferredSize.Width);
                height = Math.Max(height, preferredSize.Height);
            }

            // Each button can grow to fit its text, but using the largest
            // preferred size as the minimum keeps the button set visually uniform.
            foreach (Button button in buttons)
            {
                button.MinimumSize = new Size(width, height);
            }
        }

        private static Panel CreatePanel()
        {
            return new Panel
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.FixedSingle
            };
        }

        private Label CreateLabel(string text, float size, FontStyle style)
        {
            return new Label
            {
                Text = text,
                AutoSize = true,
                Font = new Font(Font.FontFamily, size, style)
            };
        }

        private void BtnOpenImage_Click(object? sender, EventArgs e)
        {
            // The Presentation layer asks a dialog helper for the path instead
            // of mixing OpenFileDialog setup directly into the event handler.
            string? filePath = imageFileDialog.SelectImage();

            if (filePath is null)
                return;

            // Image loading stays in the Application layer so the form only
            // coordinates UI state: what to show and where to show it.
            Bitmap image = imageService.LoadImage(filePath);

            ReplaceOriginalImage(image);
            txtImagePath.Text = filePath;
        }

        private void BtnOriginal_Click(object? sender, EventArgs e)
        {
            if (originalImage is null)
                return;

            // Restoring the original also releases any generated filter result,
            // because the PictureBox no longer needs to display it.
            pictureBoxImage.Image = originalImage;
            DisposeProcessedImage();
        }

        private void BtnGrayscale_Click(object? sender, EventArgs e)
        {
            if (originalImage is null)
                return;

            // The Application layer coordinates the Domain processor; the form
            // only requests the operation and displays the returned bitmap.
            Bitmap grayscaleImage = imageService.ApplyGrayscale(originalImage);

            ReplaceProcessedImage(grayscaleImage);
        }

        private void BtnLinearGrayscale_Click(object? sender, EventArgs e)
        {
            if (originalImage is null)
                return;

            Bitmap grayscaleImage = imageService.ApplyLinearGrayscale(originalImage);

            ReplaceProcessedImage(grayscaleImage);
        }

        private void ReplaceOriginalImage(Bitmap image)
        {
            pictureBoxImage.Image = null;
            DisposeProcessedImage();
            // Selecting a new file replaces the previous original, so the old
            // bitmap can be disposed after it is detached from the PictureBox.
            originalImage?.Dispose();

            // The original image is preserved separately so filters can be
            // applied to copies without destroying the source image.
            originalImage = image;
            pictureBoxImage.Image = originalImage;
            SetFilterButtonsEnabled(true);
        }

        private void ReplaceProcessedImage(Bitmap image)
        {
            Bitmap? previousProcessedImage = processedImage;

            processedImage = image;
            pictureBoxImage.Image = processedImage;

            // Generated images are disposable resources. Once a replacement is
            // shown, the previous generated bitmap is no longer needed.
            previousProcessedImage?.Dispose();
        }

        private void SetFilterButtonsEnabled(bool enabled)
        {
            btnOriginal.Enabled = enabled;
            btnGrayscale.Enabled = enabled;
            btnLinearGrayscale.Enabled = enabled;
        }

        private void DisposeProcessedImage()
        {
            processedImage?.Dispose();
            processedImage = null;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            // Detach the PictureBox before disposing images it may reference.
            pictureBoxImage.Image = null;
            DisposeProcessedImage();
            originalImage?.Dispose();

            base.OnFormClosed(e);
        }
    }
}
