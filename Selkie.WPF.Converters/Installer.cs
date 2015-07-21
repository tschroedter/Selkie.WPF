﻿using Castle.Core;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Selkie.Windsor;
using Selkie.WPF.Converters.Interfaces;

namespace Selkie.WPF.Converters
{
    //ncrunch: no coverage start 
    public class Installer : BaseInstaller <Installer>
    {
        protected override void InstallComponents(IWindsorContainer container,
                                                  IConfigurationStore store)
        {
            base.InstallComponents(container,
                                   store);

            container.Register(Classes.FromThisAssembly()
                                      .BasedOn <IConverter>()
                                      .WithServiceFromInterface(typeof ( IConverter ))
                                      .Configure(c => c.LifeStyle.Is(LifestyleType.Transient)));
        }
    }
}