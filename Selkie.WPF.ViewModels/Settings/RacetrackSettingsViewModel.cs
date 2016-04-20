using System;
using System.Windows.Input;
using JetBrains.Annotations;
using Selkie.EasyNetQ;
using Selkie.Windsor;
using Selkie.Windsor.Extensions;
using Selkie.WPF.Common.Interfaces;
using Selkie.WPF.Models.Common.Messages;
using Selkie.WPF.Models.Interfaces;
using Selkie.WPF.ViewModels.Interfaces;
using Selkie.WPF.ViewModels.Interfaces.Settings;

namespace Selkie.WPF.ViewModels.Settings
{
    public class RacetrackSettingsViewModel
        : ViewModel,
          IRacetrackSettingsViewModel
    {
        public enum PossibleTurns
        {
            Port,
            StarPort,
            Both
        }

        internal const int PeriodInMs = 500;
        internal const int DueTimeInMs = 1000;
        private readonly ISelkieInMemoryBus m_Bus;
        private readonly ICommandManager m_CommandManager;
        private readonly IApplicationDispatcher m_Dispatcher;
        private ICommand m_ApplyCommand;
        private double m_TurnRadiusForPort;
        private double m_TurnRadiusForStarboard;

        public RacetrackSettingsViewModel([NotNull] ISelkieLogger logger,
                                          [NotNull] ISelkieInMemoryBus bus,
                                          [NotNull] IApplicationDispatcher dispatcher,
                                          [NotNull] ICommandManager commandManager,
                                          [UsedImplicitly] IRacetrackSettingsModel model)
        {
            m_Bus = bus;
            m_Dispatcher = dispatcher;
            m_CommandManager = commandManager;

            TurnRadiusForPort = 100.0;
            TurnRadiusForStarboard = 100.0;
            AllowedTurns = PossibleTurns.Both;
            IsPortTurnAllowed = true;
            IsStarboardTurnAllowed = true;
            IsApplying = false;

            AllowedTurns = DetermineAllowedTurns(IsPortTurnAllowed,
                                                 IsStarboardTurnAllowed);

            bus.SubscribeAsync <RacetrackSettingsResponseMessage>(GetType().ToString(),
                                                                  // todo this should be Colony...message
                                                                  RacetrackSettingsResponseHandler);

            bus.PublishAsync(new RacetrackSettingsRequestMessage());
        }

        public double TurnRadiusForPort
        {
            get
            {
                return m_TurnRadiusForPort;
            }
            set
            {
                m_TurnRadiusForPort = value;
                NotifyPropertyChanged("TurnRadiusForPort");
            }
        }

        public double TurnRadiusForStarboard
        {
            get
            {
                return m_TurnRadiusForStarboard;
            }
            set
            {
                m_TurnRadiusForStarboard = value;
                NotifyPropertyChanged("TurnRadiusForStarboard");
            }
        }

        public PossibleTurns AllowedTurns { get; set; }

        public bool IsApplyEnabled
        {
            get
            {
                return !IsApplying;
            }
        }

        public bool IsPortTurnAllowed { get; set; }
        public bool IsStarboardTurnAllowed { get; set; }
        public bool IsApplying { get; private set; }

        public ICommand ApplyCommand
        {
            get
            {
                return m_ApplyCommand ?? ( m_ApplyCommand = new DelegateCommand(m_CommandManager,
                                                                                Apply,
                                                                                ApplyCommandCanExecute) );
            }
        }

        internal bool ApplyCommandCanExecute()
        {
            return !IsApplying;
        }

        internal void Apply()
        {
            IsApplying = true;

            UpdateSelectedTurns();

            var message = new RacetrackSettingsSetMessage // todo this should be Colony...message
                          {
                              TurnRadiusForPort = TurnRadiusForPort,
                              TurnRadiusForStarboard = TurnRadiusForStarboard,
                              IsPortTurnAllowed = IsPortTurnAllowed,
                              IsStarboardTurnAllowed = IsStarboardTurnAllowed
                          };

            m_Bus.PublishAsync(message);

            NotifyPropertyChanged("IsApplyEnabled");
        }

        internal PossibleTurns DetermineAllowedTurns(bool isPortTurnAllowed,
                                                     bool isStarPortTurnAllowed)
        {
            if ( isPortTurnAllowed && isStarPortTurnAllowed )
            {
                return PossibleTurns.Both;
            }

            if ( isPortTurnAllowed )
            {
                return PossibleTurns.Port;
            }

            return PossibleTurns.StarPort;
        }

        internal void RacetrackSettingsResponseHandler(RacetrackSettingsResponseMessage message)
        {
            m_Dispatcher.BeginInvoke(() => UpdateAndNotify(message));
        }

        private void UpdateAndNotify(RacetrackSettingsResponseMessage message)
        {
            Update(message.TurnRadiusForPort,
                   message.TurnRadiusForStarboard,
                   message.IsPortTurnAllowed,
                   message.IsStarboardTurnAllowed);

            IsApplying = false;

            NotifyPropertyChanged("IsApplyEnabled");

            m_CommandManager.InvalidateRequerySuggested();
        }

        internal void Update(double turnRadiusForPort,
                             double turnRadiusForStarboard,
                             bool isPortTurnAllowed,
                             bool isStarboardTurnAllowed)
        {
            TurnRadiusForPort = turnRadiusForPort;
            TurnRadiusForStarboard = turnRadiusForStarboard;
            IsPortTurnAllowed = isPortTurnAllowed;
            IsStarboardTurnAllowed = isStarboardTurnAllowed;
        }

        internal void UpdateSelectedTurns()
        {
            switch ( AllowedTurns )
            {
                case PossibleTurns.Port:
                    IsPortTurnAllowed = true;
                    IsStarboardTurnAllowed = false;
                    break;

                case PossibleTurns.StarPort:
                    IsPortTurnAllowed = false;
                    IsStarboardTurnAllowed = true;
                    break;

                case PossibleTurns.Both:
                    IsPortTurnAllowed = true;
                    IsStarboardTurnAllowed = true;
                    break;

                default:
                    throw new ArgumentException("Unknown PossibleTurns! - {0}".Inject(AllowedTurns));
            }
        }
    }
}