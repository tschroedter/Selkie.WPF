using System;
using System.Windows;
using HiveStudios.EventBroker;
using Interfaces;

namespace Main
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly IThreadsManager m_ThreadManager;
        private readonly IEventsMessages m_EventsMessages;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        public MainWindow(IAbstractFactory context)
        {
            InitializeComponent();

            try
            {
                IConfig config = context.Create<IConfig>();
                config.ReadGlobalConfigFile();

                #region Creating objects

                m_ThreadManager = context.Create<IThreadsManager>();
                m_EventsMessages = context.Create<IEventsMessages>();

                IMapView theMapView = context.Create<IMapView>();

                // Set Panel with the Map View
                IDockingCenter inductionCenter = context.Create<IDockingCenter>();
                inductionCenter.SetManager(m_DockManager);
                inductionCenter.AssignToArea(theMapView, "Dock Panel 1");

                #endregion

                #region Releasing objects

                context.Release(inductionCenter);
                context.Release(theMapView);

                #endregion

            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to create components: " +  ex.Message);
            }

        }

        /// <summary>
        /// Handles the Closing event of the Window control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.ComponentModel.CancelEventArgs"/> instance containing the event data.</param>
        private void WindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            // Send a message to all interested - that window is closing
            EventBroker.Execute(m_EventsMessages.ApplicationCloseMessage, this, null);

            try
            {
                // Wait for all threads to exit
                m_ThreadManager.ThreadsCountDown.Wait();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to terminate Threads: " + ex.Message);
            }
        }

    }
}
