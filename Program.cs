using ImageViewer.Presentation;

namespace ImageViewer
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // WinForms uses COM-based UI components such as file dialogs, which
            // require the main UI thread to run in single-threaded apartment mode.
            // This applies the framework's default WinForms startup settings,
            // such as high DPI behavior and default font configuration.
            ApplicationConfiguration.Initialize();

            // The fully qualified name avoids confusion with the project's
            // ImageViewer.Application namespace.
            System.Windows.Forms.Application.Run(new MainForm());
        }
    }
}
