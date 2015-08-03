﻿using System.Diagnostics.CodeAnalysis;
using Castle.Windsor;
using Selkie.WPF.Common.Interfaces.Windsor;

namespace Selkie.WPF.Application.Windsor
{
    [ExcludeFromCodeCoverage]
    public class WindsorViewFactory : IViewFactory
    {
        private readonly IWindsorContainer m_Container;

        public WindsorViewFactory(IWindsorContainer container)
        {
            m_Container = container;
        }

        public T CreateView <T>() where T : IView
        {
            return m_Container.Resolve <T>();
        }

        public T CreateView <T>(object argumentsAsAnonymousType) where T : IView
        {
            return m_Container.Resolve <T>(argumentsAsAnonymousType);
        }
    }
}