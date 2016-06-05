using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using JetBrains.Annotations;
using Selkie.Windsor;
using Selkie.Windsor.Extensions;
using Selkie.WPF.Common.Interfaces;
using Selkie.WPF.Common.Interfaces.Windsor;
using Xceed.Wpf.AvalonDock;
using Xceed.Wpf.AvalonDock.Layout;

namespace Selkie.WPF.Common
{
    [ExcludeFromCodeCoverage]
    [ProjectComponent(Lifestyle.Singleton)]
    public class DockingCenter : IDockingCenter
    {
        public DockingCenter()
        {
            m_DisplayMap = new Dictionary <string, LayoutAnchorablePane>();
        }

        public ISelkieLogger Logger { get; set; }
        private readonly Dictionary <string, LayoutAnchorablePane> m_DisplayMap;
        private DockingManager m_DockManager;
        private List <LayoutAnchorablePane> m_Layouts;

        public void AssignToArea([NotNull] IView view,
                                 [NotNull] string title)
        {
            if ( m_DisplayMap == null )
            {
                throw new NullReferenceException("display map not initialized yet");
            }

            if ( !m_DisplayMap.Keys.Contains(title) )
            {
                throw new Exception("area: " + title + " not exists in dock panels list");
            }

            string message = "View {0} was added to the {1} docking area".Inject(view,
                                                                                 title);
            Logger.Info(message);

            m_DisplayMap [ title ].SelectedContent.Content = view.GetContent();
        }

        public void SetManager(object manager)
        {
            if ( !( manager is DockingManager ) )
            {
                throw new Exception("This module currently supports only AvalonDock.DockingManager");
            }

            m_DockManager = ( DockingManager ) manager;
            m_Layouts =
                new List <LayoutAnchorablePane>(m_DockManager.Layout.Descendents().OfType <LayoutAnchorablePane>());

            InitializeDockingCenter();
        }

        private void AddAreaPlaceholder(string key,
                                        LayoutAnchorablePane thePlaceHolder)
        {
            if ( m_DisplayMap == null )
            {
                throw new NullReferenceException("display map not initialized yet");
            }

            if ( m_DisplayMap.Keys.Contains(key) )
            {
                throw new Exception("area placeholder already exists in dictionary");
            }

            m_DisplayMap.Add(key,
                             thePlaceHolder);
        }

        private void InitializeDockingCenter()
        {
            if ( m_DockManager == null )
            {
                throw new NullReferenceException("docking manager is not valid or null");
            }

            if ( m_DisplayMap == null )
            {
                throw new NullReferenceException("display map not initialized yet");
            }

            m_DisplayMap.Clear();

            foreach ( LayoutAnchorablePane element in m_Layouts )
            {
                AddAreaPlaceholder(element.SelectedContent.Title,
                                   element);
            }
        }
    }
}