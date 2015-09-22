using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Castle.Core;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Selkie.EasyNetQ;
using Selkie.Windsor;
using Selkie.WPF.Models.Interfaces;

namespace Selkie.WPF.Models
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

            var register = container.Resolve<IRegisterMessageHandlers>();

            register.Register(container,
                              Assembly.GetAssembly(typeof(Installer)));

            container.Release(register);

            container.Register(
                               Classes.FromThisAssembly()
                                      .BasedOn <IModel>()
                                      .WithServiceFromInterface(typeof ( IModel ))
                                      .Configure(c => c.LifeStyle.Is(LifestyleType.Transient)));
        }
    }
}