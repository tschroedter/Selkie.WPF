﻿using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Castle.Core;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Selkie.Common;
using Selkie.EasyNetQ;
using Selkie.Framework.Interfaces.Converters;

namespace Selkie.Framework
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

            var consumers = container.Resolve <IRegisterMessageHandlers>();

            consumers.Register(container,
                               Assembly.GetAssembly(typeof( Installer )));

            container.Release(consumers);
        }
    }
}