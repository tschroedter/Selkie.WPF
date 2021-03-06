﻿using System.Diagnostics.CodeAnalysis;
using Castle.Core;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Selkie.Common;
using Selkie.WPF.Common.Interfaces.Windsor;

namespace Selkie.WPF.Views
{
    [ExcludeFromCodeCoverage]
    public class Installer : SelkieInstaller <Installer>
    {
        protected override void InstallComponents(IWindsorContainer container,
                                                  IConfigurationStore store)
        {
            base.InstallComponents(container,
                                   store);

            container.Register(
                               Classes.FromThisAssembly()
                                      .BasedOn <IView>()
                                      .WithServiceFromInterface(typeof( IView ))
                                      .Configure(c => c.LifeStyle.Is(LifestyleType.Transient)));
        }
    }
}