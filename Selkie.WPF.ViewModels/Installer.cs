using System.Diagnostics.CodeAnalysis;
using Castle.Core;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Selkie.Common;
using Selkie.WPF.Common.Interfaces.Windsor;
using Selkie.WPF.ViewModels.Interfaces;
using Selkie.WPF.ViewModels.Mapping.Handlers;

namespace Selkie.WPF.ViewModels
{
    [ExcludeFromCodeCoverage]
    public class Installer : SelkieInstaller <Installer>
    {
        protected override void InstallComponents(IWindsorContainer container,
                                                  IConfigurationStore store)
        {
            base.InstallComponents(container,
                                   store);

            container.Register(Classes.FromThisAssembly()
                                      .BasedOn <IConverter>()
                                      .WithServiceFromInterface(typeof( IConverter ))
                                      .Configure(c => c.LifeStyle.Is(LifestyleType.Transient)));

            container.Register(Classes.FromThisAssembly()
                                      .BasedOn <IViewModel>()
                                      .WithServiceFromInterface(typeof( IViewModel ))
                                      .Configure(c => c.LifeStyle.Is(LifestyleType.Transient)));

            container.Register(Component.For <IMapViewModelMessageHandler>() // todo find better way
                                        .ImplementedBy <ShortestPathModelChangedHandler>());

            container.Register(Component.For <IMapViewModelMessageHandler>()
                                        .ImplementedBy <LinesModelChangedHandler>());

            container.Register(Component.For <IMapViewModelMessageHandler>()
                                        .ImplementedBy <NodesModelChangedHandler>());

            container.Register(Component.For <IMapViewModelMessageHandler>()
                                        .ImplementedBy <StartNodeModelChangeHandler>());

            container.Register(Component.For <IMapViewModelMessageHandler>()
                                        .ImplementedBy <EndNodeModelChangeHandler>());

            container.Register(Component.For <IMapViewModelMessageHandler>()
                                        .ImplementedBy <ShortestPathDirectionModelChangedHandler>());

            container.Register(Component.For <IMapViewModelMessageHandler>()
                                        .ImplementedBy <RacetrackModelChangedHandler>());
        }
    }
}