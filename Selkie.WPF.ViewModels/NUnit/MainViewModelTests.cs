using System.Diagnostics.CodeAnalysis;
using NSubstitute;
using NUnit.Framework;
using Selkie.WPF.Common.Interfaces.Windsor;
using Selkie.WPF.ViewModels.Interfaces;

namespace Selkie.WPF.ViewModels.NUnit
{
    //ncrunch: no coverage start
    [TestFixture]
    [ExcludeFromCodeCoverage]
    internal sealed class MainViewModelTests
    {
        [SetUp]
        public void Setup()
        {
            m_CommandManager = Substitute.For <ICommandManager>();

            m_Model = new MainViewModel(m_CommandManager);
        }

        private MainViewModel m_Model;
        private ICommandManager m_CommandManager;

        [Test]
        public void ClosingCommandCanExecuteTest()
        {
            Assert.True(m_Model.ClosingCommandCanExecute());
        }

        [Test]
        public void ClosingCommandTest()
        {
            Assert.NotNull(m_Model.ClosingCommand);
        }

        [Test]
        public void ParentViewRoundtripTest()
        {
            var view = Substitute.For <IMainView>();

            m_Model.ParentView = view;

            Assert.AreEqual(view,
                            m_Model.ParentView);
        }
    }
}