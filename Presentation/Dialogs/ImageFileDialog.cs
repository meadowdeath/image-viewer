namespace ImageViewer.Presentation.Dialogs
{
    public class ImageFileDialog
    {
        public string? SelectImage()
        {
            // OpenFileDialog is a presentation concern because it interacts
            // directly with the user and the operating system's file picker.
            using OpenFileDialog dialog = new OpenFileDialog
            {
                Title = "Select an image",
                // The filter limits the visible choices to common image file
                // types while still returning the full selected file path.
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp"
            };

            // DialogResult.OK means the user confirmed a selection; cancelling
            // returns null so callers can exit without treating it as an error.
            if (dialog.ShowDialog() != DialogResult.OK)
                return null;

            return dialog.FileName;
        }
    }
}
