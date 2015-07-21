using System;
using System.Reflection;
using System.Windows;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Interfaces;

namespace Main
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            RegisterUnhandledEvents();

            //Resolve our program to start the application.
            var mainProgram = Bootstrapper.Container.Resolve<IShell<MainWindow>>();
            mainProgram.Run();
            Bootstrapper.Container.Release(mainProgram);
        }

        private void RegisterUnhandledEvents()
        {
            // UI Exceptions
            this.DispatcherUnhandledException += Application_DispatcherUnhandledException;

            // Thread exceptions
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;

        }

        /// <summary>
        /// Handles the DispatcherUnhandledException event of the Application control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="System.Windows.Threading.DispatcherUnhandledExceptionEventArgs"/> instance containing the event data.</param>
        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            var exception = e.Exception;
            HandleUnhandledException(exception);
        }

        /// <summary>
        /// Currents the domain on unhandled exception.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="unhandledExceptionEventArgs">The <see cref="UnhandledExceptionEventArgs"/> instance containing the event data.</param>
        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs unhandledExceptionEventArgs)
        {
            HandleUnhandledException(unhandledExceptionEventArgs.ExceptionObject as Exception);

            if (unhandledExceptionEventArgs.IsTerminating)
            {
                MessageBox.Show("Application is terminating due to an unhandled exception in a secondary thread.");
            }
        }

        /// <summary>
        /// Handles the unhandled exception.
        /// </summary>
        /// <param name="exception">The exception.</param>
        private void HandleUnhandledException(Exception exception)
        {
            string message = "Unhandled exception";
            try
            {
                AssemblyName assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName();
                message = string.Format("Unhandled exception in {0} v{1}", assemblyName.Name, assemblyName.Version);
            }
            catch (Exception exc)
            {
                MessageBox.Show("Exception in unhandled exception handler: " + exc.Message);
            }
            finally
            {
                MessageBox.Show(message + ": " + exception.Message);
            }
        }
    }
}
