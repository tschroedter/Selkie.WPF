using System;
using System.ComponentModel;
using System.Windows;
using JetBrains.Annotations;
using Selkie.Windsor;
using Selkie.Windsor.Extensions;
using Selkie.WPF.Common.Interfaces;
using Selkie.WPF.Common.Interfaces.Windsor;
using Selkie.WPF.ViewModels.Interfaces;
using Selkie.WPF.Views.Interfaces;

namespace Selkie.WPF.Views
{
    public partial class MainView : IMainView
    {
        private readonly IMainViewModel m_MainViewModel;

        public MainView([NotNull] ISelkieLogger logger,
                        [NotNull] IMainViewModel mainViewModel,
                        [NotNull] IViewFactory viewFactory,
                        [NotNull] IDockingCenter dockingCenter)
        {
            m_MainViewModel = mainViewModel;
            InitializeComponent();

            try
            {
                var controlView = viewFactory.CreateView <IControlView>();
                var mapView = viewFactory.CreateView <IMapView>();
                var pheromonesView = viewFactory.CreateView <IPheromonesView>();
                var bestTrailsHistoryView = viewFactory.CreateView <ITrailsHistoryView>();
                var settingsView = viewFactory.CreateView <ISettingsView>();

                dockingCenter.SetManager(m_DockManager);
                dockingCenter.AssignToArea(controlView,
                                           "Control");
                dockingCenter.AssignToArea(mapView,
                                           "Preview");
                dockingCenter.AssignToArea(pheromonesView,
                                           "Pheromones");
                dockingCenter.AssignToArea(bestTrailsHistoryView,
                                           "History");
                dockingCenter.AssignToArea(settingsView,
                                           "Settings");

                Closing += MainView_Closing;
            }
            catch ( Exception exception )
            {
                string message = LogException(logger,
                                              exception);

                MessageBox.Show(message);
            }
        }

        public object GetContent()
        {
            return Content;
        }

        private void MainView_Closing(object sender,
                                      CancelEventArgs e)
        {
            m_MainViewModel.ShutDown();
        }

        private string LogException(ISelkieLogger logger,
                                    Exception exception)
        {
            string message = "Failed to create components: {0}\r\n\r\n{1}".Inject(exception.Message,
                                                                                  exception.StackTrace);

            logger.Error(message);

            if ( exception.InnerException != null )
            {
                message += LogException(logger,
                                        exception.InnerException);
            }

            return message;
        }
    }
}