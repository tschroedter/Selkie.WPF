using Interfaces;

namespace Main
{
    /// <summary>
    /// Credit to: Castle Windsor Tutorial
    /// http://app-code.net/wordpress/?p=676
    /// </summary>
    public class Shell : IShell<MainWindow>
    {
        /// <summary>
        /// Gets or sets the window.
        /// </summary>
        /// <value>
        /// The window.
        /// </value>
        public MainWindow window { get; set; }

        /// <summary>
        /// Runs this instance.
        /// </summary>
        public void Run()
        {
            window.Show();
        }
    }
}
