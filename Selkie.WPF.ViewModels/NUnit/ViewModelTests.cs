using System.Diagnostics.CodeAnalysis;
using NUnit.Framework;

namespace Selkie.WPF.ViewModels.NUnit
{
    //ncrunch: no coverage start
    [TestFixture]
    [ExcludeFromCodeCoverage]
    internal sealed class ViewModelTests
    {
        [SetUp]
        public void Setup()
        {
            m_Model = new TestViewModel();
        }

        private ViewModel m_Model;

        private class TestViewModel : ViewModel
        {
        }

        [Test]
        public void NotifyPropertyChangedTest()
        {
            var test = new TestNotifyPropertyChanged(m_Model,
                                                     "Path");

            m_Model.NotifyPropertyChanged("Path");

            Assert.True(test.IsExpectedNotified,
                        "IsExpectedNotified");
        }
    }
}