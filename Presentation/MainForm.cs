using ImageViewer.Application.Services;
using ImageViewer.Presentation.Dialogs;

namespace ImageViewer.Presentation
{
    public class MainForm : Form
    {
        private readonly Button btnOpenImage;
        private readonly PictureBox pictureBoxImage;
        private readonly TextBox txtImagePath;

        // The form coordinates the UI workflow, while these classes keep file
        // selection and image loading out of the presentation code.
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

            Controls.Add(CreateLayout());

            // This subscribes the button's Click event to the method that runs
            // the image-selection workflow when the user presses the button.
            btnOpenImage.Click += BtnOpenImage_Click;
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
                Height = 38,
                Padding = new Padding(14, 6, 14, 6)
            };
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
            Image image = imageService.LoadImage(filePath);

            pictureBoxImage.Image = image;
            txtImagePath.Text = filePath;
        }
    }
}
