using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using JetBrains.Annotations;
using Selkie.EasyNetQ;
using Selkie.Windsor;
using Selkie.WPF.Common.Interfaces;
using Selkie.WPF.Models.Common.Messages;
using Selkie.WPF.Models.Interfaces;
using Selkie.WPF.ViewModels.Interfaces;
using Selkie.WPF.ViewModels.Interfaces.Settings;

namespace Selkie.WPF.ViewModels.Settings
{
    public class AntSettingsViewModel
        : ViewModel,
          IAntSettingsViewModel
    {
        private readonly ISelkieInMemoryBus m_Bus;
        private readonly ICommandManager m_CommandManager;
        private readonly IApplicationDispatcher m_Dispatcher;
        private ICommand m_ApplyCommand;
        private bool m_IsFixedStartNode;
        private IEnumerable <IAntSettingsNode> m_Nodes;
        private IAntSettingsNode m_SelectedNode;

        public AntSettingsViewModel([NotNull] ISelkieLogger logger,
                                    [NotNull] ISelkieInMemoryBus bus,
                                    [NotNull] IApplicationDispatcher dispatcher,
                                    [NotNull] ICommandManager commandManager,
                                    [UsedImplicitly] IAntSettingsModel model)
        {
            m_Bus = bus;
            m_Dispatcher = dispatcher;
            m_CommandManager = commandManager;
            Nodes = new IAntSettingsNode[0];

            string subscriptionId = GetType().ToString();

            m_Bus.SubscribeAsync <AntSettingsModelChangedMessage>(subscriptionId,
                                                                  AntSettingsModelChangedHandler);

            m_Bus.PublishAsync(new AntSettingsModelRequestMessage());
        }

        public IEnumerable <IAntSettingsNode> Nodes
        {
            get
            {
                return m_Nodes;
            }
            private set
            {
                m_Nodes = value;

                NotifyPropertyChanged("Nodes");
            }
        }

        [CanBeNull]
        public IAntSettingsNode SelectedNode
        {
            get
            {
                return m_SelectedNode;
            }
            set
            {
                m_SelectedNode = value;

                NotifyPropertyChanged("SelectedNode");
            }
        }

        public bool IsFixedStartNode
        {
            get
            {
                return m_IsFixedStartNode;
            }
            set
            {
                m_IsFixedStartNode = value;

                NotifyPropertyChanged("IsFixedStartNode");
            }
        }

        public bool IsApplying { get; private set; }

        public bool IsApplyEnabled
        {
            get
            {
                return !IsApplying;
            }
        }

        public ICommand ApplyCommand
        {
            get
            {
                return m_ApplyCommand ?? ( m_ApplyCommand = new DelegateCommand(m_CommandManager,
                                                                                Apply,
                                                                                ApplyCommandCanExecute) );
            }
        }

        internal void AntSettingsModelChangedHandler([NotNull] AntSettingsModelChangedMessage message)
        {
            m_Dispatcher.BeginInvoke(() => UpdateAndNotify(message));
        }

        internal void UpdateAndNotify([NotNull] AntSettingsModelChangedMessage message)
        {
            Update(message.IsFixedStartNode,
                   message.FixedStartNode,
                   message.Nodes);

            IsApplying = false;

            NotifyPropertyChanged("IsApplyEnabled");
        }

        internal void Update(bool isFixedStartNode,
                             int fixedStartNode,
                             [NotNull] IEnumerable <IAntSettingsNode> nodes)
        {
            IsFixedStartNode = isFixedStartNode;

            IAntSettingsNode[] nodesArray = nodes.ToArray();

            IAntSettingsNode antSettingsNode = nodesArray.FirstOrDefault(x => x.Id == fixedStartNode) ??
                                               nodesArray.FirstOrDefault();

            SelectedNode = antSettingsNode;
            Nodes = nodesArray;
        }

        internal bool ApplyCommandCanExecute()
        {
            return !IsApplying;
        }

        internal void Apply()
        {
            IsApplying = true;

            int fixedStartNode = m_SelectedNode != null
                                     ? m_SelectedNode.Id
                                     : 0;

            var message = new AntSettingsModelSetMessage
                          {
                              IsFixedStartNode = IsFixedStartNode,
                              FixedStartNode = fixedStartNode
                          };

            m_Bus.PublishAsync(message);

            NotifyPropertyChanged("IsApplyEnabled");
        }
    }
}