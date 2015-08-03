using System.Diagnostics.CodeAnalysis;
using Castle.Core;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Selkie.Windsor;
using Selkie.WPF.Models.Interfaces;

namespace Selkie.WPF.Models
{
    //ncrunch: no coverage start 
    [ExcludeFromCodeCoverage]
    public class Installer : BaseInstaller <Installer>
    {
        protected override void InstallComponents(IWindsorContainer container,
                                                  IConfigurationStore store)
        {
            base.InstallComponents(container,
                                   store);

            container.Register(
                               Classes.FromThisAssembly()
                                      .BasedOn <IModel>()
                                      .WithServiceFromInterface(typeof ( IModel ))
                                      .Configure(c => c.LifeStyle.Is(LifestyleType.Transient)));
        }
    }
}