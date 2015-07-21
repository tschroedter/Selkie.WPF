using System;
using System.IO;
using System.Reflection;
using System.Windows;
using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Interfaces;

namespace Main
{
    public class Installers : IWindsorInstaller
    {
        /// <summary>
        /// Performs the installation in the <see cref="T:Castle.Windsor.IWindsorContainer" />.
        /// </summary>
        /// <param name="container">The container.</param>
        /// <param name="store">The configuration store.</param>
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            try
            {
                //Adding this code into the installer tells castle that we 
                //are about to specify a new type factory, we now just need 
                //to add a new line to identify the factory as shown below
                container.AddFacility<TypedFactoryFacility>();

                container.Register(Component.For<IAbstractFactory>().AsFactory());
                container.Register(Component.For<IShell<MainWindow>>().ImplementedBy<Shell>().LifestyleTransient());

                container.Register(Classes.FromAssemblyInDirectory(new AssemblyFilter(AssemblyDirectory))
                        .BasedOn<IView>()
                        .Configure(c => c.LifestyleTransient().Named(c.Implementation.Name))
                        .WithService.Base()
                        .WithService.FromInterface(typeof(IView)));

                container.Register(Classes.FromAssemblyInDirectory(new AssemblyFilter(AssemblyDirectory))
                        .BasedOn<IViewModel>()
                        .Configure(c => c.LifestyleTransient().Named(c.Implementation.Name))
                        .WithService.Base()
                        .WithService.FromInterface(typeof(IViewModel)));

                container.Register(Types.FromAssemblyInDirectory(new AssemblyFilter(AssemblyDirectory + "\\CommonData"))
                     .Where(type => type.Name.StartsWith("Common", System.StringComparison.Ordinal))
                     .WithService.AllInterfaces());

                container.Register(Types.FromAssemblyInDirectory(new AssemblyFilter(AssemblyDirectory + "\\CommonGui"))
                     .Where(type => type.Name.StartsWith("Common", System.StringComparison.Ordinal))
                     .WithService.AllInterfaces());

                container.Register(Types.FromAssemblyInDirectory(new AssemblyFilter(AssemblyDirectory + "\\CommonUtilities"))
                     .Where(type => type.Name.StartsWith("Common", System.StringComparison.Ordinal))
                     .WithService.AllInterfaces());


                container.Register(Types.FromAssemblyInDirectory(new AssemblyFilter(AssemblyDirectory + "\\Map"))
                     .Where(type => type.Name.StartsWith("Map", System.StringComparison.Ordinal))
                     .WithService.AllInterfaces());


                container.Register(Component.For<IViewFactory>().AsFactory().LifestyleTransient());
                container.Register(Component.For<MainWindow>().LifestyleTransient());

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error installing some components: " + ex.Message);
            }

        }


        /// <summary>
        /// Gets the assembly directory.
        /// </summary>
        /// <value>
        /// The assembly directory.
        /// </value>
        static public string AssemblyDirectory
        {
            get
            {
                var codeBase = Assembly.GetExecutingAssembly().CodeBase;

                var uri = new UriBuilder(codeBase);

                var path = Uri.UnescapeDataString(uri.Path);

                return Path.GetDirectoryName(path);
            }
        }
    }
}
