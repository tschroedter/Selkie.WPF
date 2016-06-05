using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;
using NSubstitute;
using NUnit.Framework;
using Selkie.EasyNetQ;
using Selkie.Windsor;
using Selkie.WPF.Common.Interfaces;
using Selkie.WPF.Models.Common.Messages;
using Selkie.WPF.Models.Interfaces;
using Selkie.WPF.ViewModels.Interfaces;
using Selkie.WPF.ViewModels.Settings;

namespace Selkie.WPF.ViewModels.Tests.Settings
{
    [TestFixture]
    [ExcludeFromCodeCoverage]
    internal sealed class RacetrackSettingsViewModelTests
    {
        [SetUp]
        public void Setup()
        {
            m_Logger = Substitute.For <ISelkieLogger>();
            m_Bus = Substitute.For <ISelkieInMemoryBus>();
            m_Dispatcher = new TestImmediateDispatcher();
            m_Manager = Substitute.For <ICommandManager>();

            m_Model = new RacetrackSettingsViewModel(m_Logger,
                                                     m_Bus,
                                                     m_Dispatcher,
                                                     m_Manager,
                                                     Substitute.For <IRacetrackSettingsModel>());
        }

        private RacetrackSettingsViewModel m_Model;
        private ISelkieLogger m_Logger;
        private IApplicationDispatcher m_Dispatcher;
        private ICommandManager m_Manager;
        private ISelkieInMemoryBus m_Bus;

        private void SetModelPropertiesToSettingsModel()
        {
            m_Model.TurnRadiusForPort = 100.0;
            m_Model.TurnRadiusForStarboard = 200.0;
            m_Model.IsPortTurnAllowed = true;
            m_Model.IsStarboardTurnAllowed = true;
        }

        private static RacetrackSettingsResponseMessage CreateRacetrackSettingsResponseMessage()
        {
            return new RacetrackSettingsResponseMessage
                   {
                       TurnRadiusForPort = 200.0,
                       TurnRadiusForStarboard = 300.0,
                       IsPortTurnAllowed = false,
                       IsStarboardTurnAllowed = true
                   };
        }

        [Test]
        public void AllowedTurns_ReturnsDefault()
        {
            Assert.AreEqual(RacetrackSettingsViewModel.PossibleTurns.Both,
                            m_Model.AllowedTurns);
        }

        [Test]
        public void AllowedTurns_Roundtrip()
        {
            // Arrange
            m_Model.AllowedTurns = RacetrackSettingsViewModel.PossibleTurns.Port;

            // Act
            RacetrackSettingsViewModel.PossibleTurns actual = m_Model.AllowedTurns;

            // Assert
            Assert.AreEqual(RacetrackSettingsViewModel.PossibleTurns.Port,
                            actual);
        }

        [Test]
        public void Apply_RaisesEventIsApplyEnabled_WhenCalled()
        {
            // Arrange
            var test = new TestNotifyPropertyChanged(m_Model,
                                                     "IsApplyEnabled");
            SetModelPropertiesToSettingsModel();

            // Act
            m_Model.Apply();

            // Assert
            Assert.True(test.IsExpectedNotified,
                        "IsApplyEnabled");
        }

        [Test]
        public void Apply_SendsMessage_WhenCalled()
        {
            // Arrange
            SetModelPropertiesToSettingsModel();

            // Act
            m_Model.Apply();

            // Assert
            m_Bus.Received()
                 .PublishAsync(
                               Arg.Is <RacetrackSettingsSetMessage>(
                                                                    x =>
                                                                    Math.Abs(x.TurnRadiusForPort - 100.0) <
                                                                    double.Epsilon &&
                                                                    Math.Abs(x.TurnRadiusForStarboard - 200.0) <
                                                                    double.Epsilon &&
                                                                    x.IsPortTurnAllowed &&
                                                                    x.IsStarboardTurnAllowed));
        }

        [Test]
        public void Apply_SetsIsApplyingToTrue_WhenCalled()
        {
            // Arrange
            // Act
            m_Model.Apply();

            // Assert
            Assert.True(m_Model.IsApplying);
        }

        [Test]
        public void ApplyCommand_ReturnsCommand_WhenCalled()
        {
            // Arrange
            // Act
            ICommand actual = m_Model.ApplyCommand;

            // Assert
            Assert.NotNull(actual);
        }

        [Test]
        public void ApplyCommandCanExecute_ReturnsFalse_ForIsApplyingIsTrue()
        {
            // Arrange
            m_Model.Apply();

            // Act
            bool actual = m_Model.ApplyCommandCanExecute();

            // Assert
            Assert.False(actual);
        }

        [Test]
        public void ApplyCommandCanExecute_ReturnsTrue_ForIsApplyingIsFalse()
        {
            // Arrange
            // Act
            bool actual = m_Model.ApplyCommandCanExecute();

            // Assert
            Assert.True(actual);
        }

        [Test]
        public void Constructor_SendsRequestMessage_WhenCalled()
        {
            // Arrange
            // Act
            // Assert
            m_Bus.Received().PublishAsync(Arg.Any <RacetrackSettingsRequestMessage>());
        }

        [Test]
        public void DefaultIsPortTurnAllowedTest()
        {
            Assert.True(m_Model.IsPortTurnAllowed);
        }

        [Test]
        public void DetermineAllowedTurns_ReturnsBoth_ForBothIsTrue()
        {
            // Arrange
            // Act
            RacetrackSettingsViewModel.PossibleTurns actual = m_Model.DetermineAllowedTurns(true,
                                                                                            true);

            // Assert
            Assert.AreEqual(RacetrackSettingsViewModel.PossibleTurns.Both,
                            actual);
        }

        [Test]
        public void DetermineAllowedTurns_ReturnsPort_ForPortIsTrue()
        {
            // Arrange
            // Act
            RacetrackSettingsViewModel.PossibleTurns actual = m_Model.DetermineAllowedTurns(true,
                                                                                            false);

            // Assert
            Assert.AreEqual(RacetrackSettingsViewModel.PossibleTurns.Port,
                            actual);
        }

        [Test]
        public void DetermineAllowedTurns_ReturnsStarPort_ForStarPortIsTrue()
        {
            // Arrange
            // Act
            RacetrackSettingsViewModel.PossibleTurns actual = m_Model.DetermineAllowedTurns(false,
                                                                                            true);

            // Assert
            Assert.AreEqual(RacetrackSettingsViewModel.PossibleTurns.StarPort,
                            actual);
        }

        [Test]
        public void IsApplying_ReturnsDefaultValue_WhenCalled()
        {
            // Arrange
            // Act
            // Assert
            Assert.False(m_Model.IsApplying);
        }

        [Test]
        public void IsPortTurnAllowed_Roundtrip()
        {
            // Arrange
            m_Model.IsPortTurnAllowed = false;

            // Act
            bool actual = m_Model.IsPortTurnAllowed;

            // Assert
            Assert.False(actual);
        }

        [Test]
        public void IsStarboardTurnAllowed_ReturnsDefault_WhenCalled()
        {
            Assert.True(m_Model.IsStarboardTurnAllowed);
        }

        [Test]
        public void IsStarboardTurnAllowed_Roundtrip()
        {
            // Arrange
            m_Model.IsStarboardTurnAllowed = false;

            // Act
            bool actual = m_Model.IsStarboardTurnAllowed;

            // Assert
            Assert.False(actual);
        }

        [Test]
        public void IsViewDifferentFromModel_ReturnsFalse_ForNotDifferent()
        {
            // Arrange
            SetModelPropertiesToSettingsModel();

            // Act
            bool actual = m_Model.IsApplying;

            // Assert
            Assert.False(actual);
        }

        [Test]
        public void IsViewDifferentFromModel_ReturnsTrue_ForIsPortTurnAllowedDifferent()
        {
            // Arrange
            SetModelPropertiesToSettingsModel();

            // Act
            m_Model.Apply();

            // Assert
            Assert.True(m_Model.IsApplying);
        }

        [Test]
        public void RacetrackSettingsResponseHandler_RaisesEventIsApplyEnabled_WhenCalled()
        {
            // Arrange
            var test = new TestNotifyPropertyChanged(m_Model,
                                                     "IsApplyEnabled");
            RacetrackSettingsResponseMessage message = CreateRacetrackSettingsResponseMessage();

            // Act
            m_Model.RacetrackSettingsResponseHandler(message);


            // Assert
            Assert.True(test.IsExpectedNotified,
                        "IsApplyEnabled");
        }

        [Test]
        public void RacetrackSettingsResponseHandler_SetsIsApplyingToFalse_WhenCalled()
        {
            // Arrange
            RacetrackSettingsResponseMessage message = CreateRacetrackSettingsResponseMessage();

            // Act
            m_Model.RacetrackSettingsResponseHandler(message);


            // Assert
            Assert.False(m_Model.IsApplying);
        }

        [Test]
        public void SettingsModelChangedHandler_CallsUpdate_WhenCalled()
        {
            // Arrange
            RacetrackSettingsResponseMessage message = CreateRacetrackSettingsResponseMessage();

            // Act
            m_Model.RacetrackSettingsResponseHandler(message);

            // Assert
            Assert.True(Math.Abs(m_Model.TurnRadiusForPort - 200.0) < double.Epsilon,
                        "TurnRadiusForPort");
            Assert.True(Math.Abs(m_Model.TurnRadiusForStarboard - 300.0) < double.Epsilon,
                        "TurnRadiusForStarboard");
            Assert.False(m_Model.IsPortTurnAllowed,
                         "IsPortTurnAllowed");
            Assert.True(m_Model.IsStarboardTurnAllowed,
                        "IsStarboardTurnAllowed");
        }

        [Test]
        public void TurnRadiusForPort_RaisesEvent_WhenChanged()
        {
            // Arrange
            var test = new TestNotifyPropertyChanged(m_Model,
                                                     "TurnRadiusForPort");
            SetModelPropertiesToSettingsModel();

            // Act
            m_Model.TurnRadiusForPort = 123.0;

            // Assert
            Assert.True(test.IsExpectedNotified,
                        "TurnRadiusForPort");
        }

        [Test]
        public void TurnRadiusForPort_ReturnsDefault_WhenCalled()
        {
            Assert.AreEqual(100.0,
                            m_Model.TurnRadiusForPort);
        }

        [Test]
        public void TurnRadiusForPort_Roundtrip()
        {
            // Arrange
            m_Model.TurnRadiusForPort = 200.0;

            // Act
            double actual = m_Model.TurnRadiusForPort;

            // Assert
            Assert.AreEqual(200.0,
                            actual);
        }

        [Test]
        public void TurnRadiusForStarboard_RaisesEvent_WhenChanged()
        {
            // Arrange
            var test = new TestNotifyPropertyChanged(m_Model,
                                                     "TurnRadiusForStarboard");
            SetModelPropertiesToSettingsModel();

            // Act
            m_Model.TurnRadiusForStarboard = 123.0;

            // Assert
            Assert.True(test.IsExpectedNotified,
                        "TurnRadiusForStarboard");
        }

        [Test]
        public void TurnRadiusForStarboard_ReturnsDefault_WhenCalled()
        {
            Assert.AreEqual(100.0,
                            m_Model.TurnRadiusForStarboard);
        }

        [Test]
        public void TurnRadiusForStarboard_Roundtrip()
        {
            // Arrange
            m_Model.TurnRadiusForStarboard = 200.0;

            // Act
            double actual = m_Model.TurnRadiusForStarboard;

            // Assert
            Assert.AreEqual(200.0,
                            actual);
        }

        [Test]
        public void Update_SetsIsPortTurnAllowed_WhenCalled()
        {
            // Arrange
            // Act
            m_Model.Update(200.0,
                           300.0,
                           false,
                           true);

            // Assert
            Assert.False(m_Model.IsPortTurnAllowed);
        }

        [Test]
        public void Update_SetsIsStarPortTurnAllowed_WhenCalled()
        {
            // Arrange
            // Act
            m_Model.Update(200.0,
                           300.0,
                           true,
                           false);

            // Assert
            Assert.False(m_Model.IsStarboardTurnAllowed);
        }

        [Test]
        public void Update_SetsTurnRadiusForPort_WhenCalled()
        {
            // Arrange
            // Act
            m_Model.Update(200.0,
                           300.0,
                           true,
                           true);

            // Assert
            Assert.AreEqual(200.0,
                            m_Model.TurnRadiusForPort);
        }

        [Test]
        public void Update_SetsTurnRadiusForStarboard_WhenCalled()
        {
            // Arrange
            // Act
            m_Model.Update(200.0,
                           300.0,
                           true,
                           true);

            // Assert
            Assert.AreEqual(300.0,
                            m_Model.TurnRadiusForStarboard);
        }

        [Test]
        public void UpdateSelectedTurns_SetsBothToTrue_ForBoth()
        {
            m_Model.AllowedTurns = RacetrackSettingsViewModel.PossibleTurns.Both;

            // Act
            m_Model.UpdateSelectedTurns();

            // Assert
            Assert.True(m_Model.IsPortTurnAllowed,
                        "IsPortTurnAllowed");
            Assert.True(m_Model.IsStarboardTurnAllowed,
                        "IsStarboardTurnAllowed");
        }

        [Test]
        public void UpdateSelectedTurns_SetsPortToTrue_ForPort()
        {
            // Arrange
            m_Model.AllowedTurns = RacetrackSettingsViewModel.PossibleTurns.Port;

            // Act
            m_Model.UpdateSelectedTurns();

            // Assert
            Assert.True(m_Model.IsPortTurnAllowed,
                        "IsPortTurnAllowed");
            Assert.False(m_Model.IsStarboardTurnAllowed,
                         "IsStarboardTurnAllowed");
        }

        [Test]
        public void UpdateSelectedTurns_SetsStarboardToTrue_ForStarboard()
        {
            // Arrange
            m_Model.AllowedTurns = RacetrackSettingsViewModel.PossibleTurns.StarPort;

            // Act
            m_Model.UpdateSelectedTurns();

            // Assert
            Assert.False(m_Model.IsPortTurnAllowed,
                         "IsPortTurnAllowed");
            Assert.True(m_Model.IsStarboardTurnAllowed,
                        "IsStarboardTurnAllowed");
        }
    }
}