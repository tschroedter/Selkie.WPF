using System.Diagnostics.CodeAnalysis;
using Castle.Core;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Selkie.Windsor;
using Selkie.WPF.Common.Interfaces.Windsor;

namespace Selkie.WPF.Views
{
    //ncrunch: no coverage start
    [ExcludeFromCodeCoverage]
    public class Installer : BaseInstaller <Installer>
    {
        public override string GetPrefixOfDllsToInstall()
        {
            return "Selkie.";
        }

        protected override void InstallComponents(IWindsorContainer container,
                                                  IConfigurationStore store)
        {
            base.InstallComponents(container,
                                   store);

            container.Register(
                               Classes.FromThisAssembly()
                                      .BasedOn <IView>()
                                      .WithServiceFromInterface(typeof ( IView ))
                                      .Configure(c => c.LifeStyle.Is(LifestyleType.Transient)));
        }
    }
}