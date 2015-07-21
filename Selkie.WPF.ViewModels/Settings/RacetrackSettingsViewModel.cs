using System;
using System.Windows.Input;
using Castle.Core.Logging;
using EasyNetQ;
using JetBrains.Annotations;
using Selkie.EasyNetQ.Extensions;
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
        };

        internal const int PeriodInMs = 500;
        internal const int DueTimeInMs = 1000;
        private readonly IBus m_Bus;
        private readonly ICommandManager m_CommandManager;
        private readonly IApplicationDispatcher m_Dispatcher;
        private ICommand m_ApplyCommand;
        private double m_TurnRadius;

        public RacetrackSettingsViewModel([NotNull] ILogger logger,
                                          [NotNull] IBus bus,
                                          [NotNull] IApplicationDispatcher dispatcher,
                                          [NotNull] ICommandManager commandManager,
                                          [UsedImplicitly] IRacetrackSettingsModel model)
        {
            m_Bus = bus;
            m_Dispatcher = dispatcher;
            m_CommandManager = commandManager;

            TurnRadius = 100.0;
            AllowedTurns = PossibleTurns.Both;
            IsPortTurnAllowed = true;
            IsStarboardTurnAllowed = true;
            IsApplying = false;

            AllowedTurns = DetermineAllowedTurns(IsPortTurnAllowed,
                                                 IsStarboardTurnAllowed);

            bus.SubscribeHandlerAsync <RacetrackSettingsChangedMessage>(logger,
                                                                        GetType().ToString(),
                                                                        RacetrackSettingsChangedHandler);

            bus.PublishAsync(new RacetrackSettingsRequestMessage());
        }

        public double TurnRadius
        {
            get
            {
                return m_TurnRadius;
            }
            set
            {
                m_TurnRadius = value;
                NotifyPropertyChanged("TurnRadius");
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

            var message = new RacetrackSettingsSetMessage
                          {
                              TurnRadius = TurnRadius,
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

        internal void RacetrackSettingsChangedHandler(RacetrackSettingsChangedMessage message)
        {
            m_Dispatcher.BeginInvoke(() => UpdateAndNotify(message));
        }

        private void UpdateAndNotify(RacetrackSettingsChangedMessage message)
        {
            Update(message.TurnRadius,
                   message.IsPortTurnAllowed,
                   message.IsStarboardTurnAllowed);

            IsApplying = false;

            NotifyPropertyChanged("IsApplyEnabled");

            m_CommandManager.InvalidateRequerySuggested();
        }

        internal void Update(double turnRadius,
                             bool isPortTurnAllowed,
                             bool isStarboardTurnAllowed)
        {
            TurnRadius = turnRadius;
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