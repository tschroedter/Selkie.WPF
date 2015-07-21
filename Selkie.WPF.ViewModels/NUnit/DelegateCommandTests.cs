using System;
using System.Diagnostics.CodeAnalysis;
using NSubstitute;
using NUnit.Framework;
using Selkie.WPF.ViewModels.Interfaces;

namespace Selkie.WPF.ViewModels.NUnit
{
    //ncrunch: no coverage start
    [TestFixture]
    [ExcludeFromCodeCoverage]
    internal sealed class DelegateCommandTests
    {
        [SetUp]
        public void Setup()
        {
            m_CommandManager = Substitute.For <ICommandManager>();

            m_Command = new DelegateCommand(m_CommandManager,
                                            ExecuteMethod,
                                            CanExecuteMethod);
        }

        private DelegateCommand m_Command;
        private bool m_IsExecuted;
        private ICommandManager m_CommandManager;

        private void OnCanExecuteChanged(object sender,
                                         EventArgs e)
        {
        }

        private void ExecuteMethod()
        {
            m_IsExecuted = true;
        }

        private bool CanExecuteMethod()
        {
            return true;
        }

        [Test]
        public void CanExecuteChangedTest()
        {
            m_Command.CanExecuteChanged += OnCanExecuteChanged;
            // note don't know how to test this
            m_Command.CanExecuteChanged -= OnCanExecuteChanged;
        }

        [Test]
        public void CanExecuteTest()
        {
            Assert.True(m_Command.CanExecute(null));
        }

        [Test]
        public void ConstructorWithOnlyExecuteMethodForCanExecuteTest()
        {
            var command = new DelegateCommand(m_CommandManager,
                                              ExecuteMethod);

            Assert.True(command.CanExecute(),
                        "CanExecute");
        }

        [Test]
        public void ConstructorWithOnlyExecuteMethodForExecuteTest()
        {
            var command = new DelegateCommand(m_CommandManager,
                                              ExecuteMethod);

            command.Execute();

            Assert.True(m_IsExecuted,
                        "IsExecuted");
        }

        [Test]
        public void ExecuteTest()
        {
            m_Command.Execute(null);

            Assert.True(m_IsExecuted);
        }

        [Test]
        public void OnCanExecuteChangedTest()
        {
            m_Command.OnCanExecuteChanged();

            m_CommandManager.Received().InvalidateRequerySuggested();
        }
    }
}